// This file is sent to browser to be compiled
class Base64
{
    static #textEncoder = new TextEncoder();
    static #textDecoder = new TextDecoder();

    // https://datatracker.ietf.org/doc/html/rfc4648#section-4
    encode = (str) => btoa(String.fromCharCode(...Base64.#textEncoder.encode(str)));
    decode = (str) => Base64.#textDecoder.decode(Uint8Array.from(atob(str), c => c.charCodeAt(0)));
    // https://datatracker.ietf.org/doc/html/rfc4648#section-5
    encodeUrl = (str) => this.encode(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    decodeUrl = (str) => this.decode(str.replace(/\-/g, '+').replace(/\_/g, '/'));

    jwtEncodeBody = (header, payload) => this.encodeUrl(JSON.stringify(header)) + '.' + this.encodeUrl(JSON.stringify(payload));
    jwtDecodePayload = (jwt) => JSON.parse(this.decodeUrl(jwt.split('.')[1]));
}

document.addEventListener('DOMContentLoaded', () =>
{
    const modal = document.getElementById('authModal');
    if (!modal) throw 'Element #authModal was not found';
    modal.addEventListener('hidden.bs.modal', event => {
        for (let input of modal.querySelectorAll('[name="user-login"], [name="user-password"]'))
        {
            const alertDiv = document.getElementById('login-alert');
            if (!alertDiv) throw 'Element #login-alert was not found';

            input.value = '';
            input.classList.remove('is-valid');
            input.classList.remove('is-invalid');
            input.nextElementSibling.innerHTML = '';
            alertDiv.style.display = 'none';
        }
    });
});

document.addEventListener('submit', e => {
    const form = e.target;
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$/;
    //@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$"
    if (form.id == 'sign-in-form') {
        e.preventDefault();
        const loginInput = form.querySelector('[name="user-login"]');
        if (!loginInput) {
            throw `Element [name="user-login"] was not found`;
        }
        const passwordInput = form.querySelector('[name="user-password"]');
        if (!passwordInput) {
            throw `Element [name="user-password"] was not found`;
        }
        if (loginInput.value.length == 0) {
            loginInput.classList.add('is-invalid');
            loginInput.nextElementSibling.innerHTML = 'Login cannot be empty';
            //return;
        }
        else {
            loginInput.classList.remove('is-invalid');
            loginInput.classList.add('is-valid');
            loginInput.nextElementSibling.innerHTML = '';
        }


        if (passwordInput.value.length == 0) {
            passwordInput.classList.add('is-invalid');
            passwordInput.nextElementSibling.innerHTML = 'Password cannot be empty';
            //return;
        }
        else {
            if (!(passwordRegex.test(passwordInput.value))) {
                passwordInput.classList.add('is-invalid');
                passwordInput.nextElementSibling.innerHTML = 'Password must be at least 12 characters long and contain lower, upper case letters, at least one number and at least one special character';
                //return;
            }
            else {
                passwordInput.classList.remove('is-invalid');
                passwordInput.classList.add('is-valid');
                passwordInput.nextElementSibling.innerHTML = '';
            }
        }
        const credentials = new Base64().encode(`${loginInput.value}:${passwordInput.value}`);
        fetch('/User/SignIn', {
            method: 'GET',
            headers: {
                'Authorization': `Basic ${credentials}`
            }
        }).then(r => r.json())
            .then(j => {
                if (j.status == 200) {
                    window.location.reload();
                }
                else {
                    const alertDiv = document.getElementById('login-alert');
                    if (!alertDiv) throw 'Element #login-alert was not found';
                    alertDiv.innerText = j.data;
                    alertDiv.style.display = '';
                }
            });
        //console.log(loginInput.value, passwordInput.value);
    }
});

document.addEventListener('DOMContentLoaded', () =>
{
    for (let btn of document.querySelectorAll('[data-nav]'))
    {
        btn.onclick = navigate;
    }

    const editProfileButton = document.getElementById('edit-profile-button');
    if (editProfileButton)
    {
        editProfileButton.addEventListener('click', editProfileButtonClick)
    }

    const deleteProfileButton = document.getElementById('delete-profile-button');
    if (deleteProfileButton)
    {
        deleteProfileButton.addEventListener('click', deleteProfileButtonClick)
    }
});

function deleteProfileButtonClick()
{
    if (confirm(`Confirm account deletion`))
    {
        let login = prompt("Enter login to confirm: ");
        if (!login || !(login.trim()))
        {
            alert("Deletion cancelled");
            return;
        }
        fetch("/User/Delete",
            {
                method: 'DELETE',
                headers: {
                    'Authentication-Control': new Base64().encodeUrl(login)
                }
            }).then(r => r.json()).then(j =>
            {
                console.log(j);
                if (j.status == 200) {
                    alert("Your profiles was deleted");
                    window.location = '/';
                }
                else
                {
                    alert("Deletion cancelled: login might be incorrect");
                }
            });
    }
}

function editProfileButtonClick()
{
    let changes = [];
    for (let element of document.querySelectorAll('[data-editable]'))
    {
        if (element.getAttribute('contenteditable'))
        {
            element.removeAttribute('contenteditable');
            //console.log(element.originalData, element.innerText);
            if (element.originalData != element.innerText)
            {
                changes.push({
                    field: element.getAttribute('data-editable'),
                    value: element.innerText
                });
            }
        }
        else
        {
            element.setAttribute('contenteditable', true);
            element.originalData = element.innerText;
        }
    }
    if (changes.length > 0)
    {
        const message = changes.map(c => `${c.field}=${c.value}`).join(', ');
        if (confirm(`Confirm changes ${message}`))
        {
            fetch("/User/Update", {
                method: 'PATCH',
                header: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(changes)
            }).then(r => r.json()).then(console.log);
        }
    }
}

function navigate(e)
{
    const targetBtn = e.target.closest('[data-nav]');
    const route = targetBtn.getAttribute('data-nav');
    if (!route) throw "Attribute [data-nav] was not found";
    
    for (let btn of document.querySelectorAll('[data-nav]'))
    {
        btn.classList.remove('active');
    }
    targetBtn.classList.add('active');

    showPage(route);
}

const authHtml = `<div>
    <div class="input-group mb-3">
          <span class="input-group-text" id="user-login-addon"><i class="bi bi-key"></i></span>
          <input name="user-login" type="text" class="form-control" placeholder="Login"
                 aria-label="Userlogin" aria-describedby="user-login-addon" value="">
          <div class="invalid-feedback"></div>
      </div>
      <div class="input-group mb-3">
          <span class="input-group-text" id="user-password-addon"><i class="bi bi-lock"></i></span>
          <input name="user-password" type="password" class="form-control" placeholder="Password"
                 aria-label="Userpassword" aria-describedby="user-password-addon">
          <div class="invalid-feedback"></div>
    </div>
    <button type="submit" class="btn btn-primary" onclick="authClick()">Log In</button>
</div>`;
const profileHtml = `<div>
    <h3>Welcome</h3>
    <button type="button" class="btn btn-dark" onclick="emailClick()">Send Email</button>
    <button type="button" class="btn btn-danger" onclick="exitClick()">Exit</button>
</div>`;

function exitClick()
{
    window.accessToken = null;
    showPage(window.activePage);
}
function emailClick()
{
    fetch("/User/Email", {
        method: "POST",
        headers: {
            "Authorization": "Bearer " + window.accessToken // window.accessToken.jti
        }
    }).then(r => r.json())
        .then(console.log);
}

function authClick()
{
    const login = document.querySelector('input[name="user-login"]').value;
    const password = document.querySelector('input[name="user-password"]').value;

    const credentials = new Base64().encode(`${login}:${password}`);
    fetch('/User/LogIn', {
        method: 'GET',
        headers: {
            'Authorization': `Basic ${credentials}`
        }
    }).then(r => r.json())
        .then(j => {
            if (j.status == 200)
            {
                window.accessToken = j.data;
                console.log(window.accessToken);

                //setTimeout(() => {
                //    exitClick();
                //    const sessionModal = new bootstrap.Modal('#session-expired-modal');
                //    sessionModal.show();
                //}, ((window.accessToken.exp - window.accessToken.iat) * 1000));

                showPage(window.activePage);
            }
            else
            {
                alert('Rejected');
            }
        });
}

function showPage(page)
{
    window.activePage = page;
    const spaContainer = document.getElementById('spa-container');
    if (!spaContainer) throw "Element #spa-container was not found";
    switch (page) {
        case 'home':    spaContainer.innerHTML = `<b>Home</b>`;     break;
        case 'privacy': spaContainer.innerHTML = `<b>Privacy</b>`;  break;
        case 'auth':    spaContainer.innerHTML = !!window.accessToken ? profileHtml : authHtml; break;
        default:        spaContainer.innerHTML = `<b>404</b>`;
    }
}
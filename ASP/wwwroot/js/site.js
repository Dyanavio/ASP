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
    modal.addEventListener('hidden.bs.modal', event => {
        for (let input of modal.querySelectorAll('[name="user-login"], [name="user-password"]'))
        {
            input.value = '';
            input.classList.remove('is-valid');
            input.classList.remove('is-invalid');
            input.nextElementSibling.innerHTML = '';
        }
    });
});


document.addEventListener('submit', e =>
{
    const form = e.target;
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$/;
    //@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$"
    if (form.id == 'sign-in-form')
    {
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
            return;
        }
        loginInput.classList.remove('is-invalid');
        loginInput.classList.add('is-valid');
        loginInput.nextElementSibling.innerHTML = '';

        if (passwordInput.value.length == 0) {
            passwordInput.classList.add('is-invalid');
            passwordInput.nextElementSibling.innerHTML = 'Password cannot be empty';
            return;
        }
        else
        {
            if (!(passwordRegex.test(passwordInput.value)))
            {
                console.log(passwordInput.value);
                passwordInput.classList.add('is-invalid');
                passwordInput.nextElementSibling.innerHTML = 'Password must be at least 12 characters long and contain lower, upper case letters, at least one number and at least one special character';
                return;
            }
        }
        passwordInput.classList.remove('is-invalid');
        passwordInput.classList.add('is-valid');
        passwordInput.nextElementSibling.innerHTML = '';

        console.log(loginInput.value, passwordInput.value);
    }
})
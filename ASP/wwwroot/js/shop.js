document.addEventListener('submit', e => {
    const form = e.target;
    if (form.id == 'product-add-form') {
        e.preventDefault();
        handleAddProduct(form);
    }
    if (form.id == 'group-add-form') {
        e.preventDefault();
        handleAddGroup(form);
    }
});

document.addEventListener('DOMContentLoaded', e => {
    for (let button of document.querySelectorAll("[data-product-id]")) {
        button.addEventListener('click', addToCartClick);
    }
    const reloadButton = document.getElementById('error-modal-reload-button');
    if (reloadButton) {
        reloadButton.onclick = reloadModalClick;
    }
});

function reloadModalClick() {
    window.location.reload();
}

function handleAddProduct(form)
{
    fetch(form.action, {
        method: 'POST',
        body: new FormData(form)
    }).then(r => r.json()).then(j =>
    {
        var success = document.getElementById('product-form-success-alert');
        if (!success) throw "Element 'product-form-success-alert' was not found";

        var fail = document.getElementById('product-form-fail-alert');
        if (!fail) throw "Element 'product-form-success-alert' was not found";

        console.log(j);
        if (j.status.statusCode == 201)
        {
            form.reset();
            fail.classList.add('d-none');
            fail.innerText = '';
            success.classList.remove('d-none');
        }
        else
        {
            success.classList.add('d-none');
            fail.classList.remove('d-none');
            fail.innerText = j.name;
        }
    });
}

function handleAddGroup(form)
{
    fetch(form.action, {
        method: 'POST',
        body: new FormData(form)
    }).then(r => r.json()).then(j =>
    {
        var success = document.getElementById('group-form-success-alert');
        if (!success) throw "Element 'group-form-success-alert' was not found";

        var fail = document.getElementById('group-form-fail-alert');
        if (!fail) throw "Element 'group-form-success-alert' was not found";

        console.log(j);
        if (j.status.statusCode == 201) // Created
        {
            form.reset();
            fail.classList.add('d-none');
            fail.innerText = '';
            success.classList.remove('d-none');
        }
        else {
            success.classList.add('d-none');
            fail.classList.remove('d-none');
            fail.innerText = j.name;
        }
    });
}


function addToCartClick(e)
{
    const button = e.target.closest("[data-product-id]");
    if (!button) throw `Closest element "[data-product-id]" was not found`;
    const productId = button.getAttribute("data-product-id");

    fetch("/api/cart/" + productId, {
        method: 'POST'
    }).then(r => r.json()).then(j =>
    {
        console.log(j);

        if (j.status.statusCode == 200) {
            // ...
        }
        else
        {
            if (j.status.statusCode == 401) {
                if (confirm("Log in to use cart. Do you want to proceed?")) {
                    const sessionModal = new bootstrap.Modal('#authModal');
                    sessionModal.show();
                }
            }
            else {
                const errorModal = new bootstrap.Modal("#error-modal");
                errorModal.show();
            }
        }
    });
}
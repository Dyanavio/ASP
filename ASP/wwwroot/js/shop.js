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
    for (let button of document.querySelectorAll("[data-cart-product-id]"))
    {
        button.addEventListener('click', modifyCartQuantity);
    }
    for (let button of document.querySelectorAll("[data-cart-product-to-delete-id]"))
    {
        button.addEventListener('click', removeFromCartClick)
    }
    const reloadButton = document.getElementById('error-modal-reload-button');
    if (reloadButton) {
        reloadButton.onclick = reloadModalClick;
    }

    if (window.location.hash == "#reload") {
        const toast = bootstrap.Toast.getOrCreateInstance(document.getElementById('remove-from-cart-toast'));
        toast.show();
        window.location.hash = "";
    }

    let buttonDiscard = document.getElementById('button-discard-cart');
    if (buttonDiscard) { buttonDiscard.onclick = showDiscardCartModal; }

    let buttonCheckout = document.getElementById('button-checkout-cart');
    if (buttonCheckout) { buttonCheckout.onclick = checkoutCartClick; }
    
    const button = document.getElementById('discard-cart-button');
    if (button) button.onclick = discardCartClick;
   

});

function showDiscardCartModal()
{
    const p = document.getElementById('delete-cart-items-message');
    p.innerText = `You are about to delete your cart and all items in it: ${document.getElementById('total-quantity').innerText} items for ${document.getElementById('total-price').innerText}. Do you want to proceed?`;

    const discardModal = new bootstrap.Modal('#delete-cart-modal');
    discardModal.show();
}

function discardCartClick()
{
    fetch("/api/cart", {
        method: 'Delete'
    }).then(r => r.json()).then(j => {
        if (j.status.isOk) {
            alert("Cart deleted");
            window.location = "/Shop";
        }
        else {
            alert(j.data);
        }
    });
    
}

function checkoutCartClick()
{
    if (confirm(`Your purchase: ${document.getElementById('total-quantity').innerText} for ${document.getElementById('total-price').innerText}`))
    {
        fetch("/api/cart", {
            method: 'PUT'
        }).then(r => r.json()).then(j => {
            if (j.status.isOk) {
                alert("Thanks for the purchase (money)");
                window.location = "/Shop";
            }
            else {
                alert(j.data);
            }
        });
    }
    
}

function removeFromCartClick(e)
{
    const button = e.target.closest("[data-cart-product-to-delete-id]");
    if (!button) throw `Closest element "[data-cart-product-to-delete-id]" was not found`;
    const productId = button.getAttribute('data-cart-product-to-delete-id');

    if (confirm('Do you want to remove this item from your cart?'))
    {
        fetch(`/api/cart/${productId}`, {
            method: 'DELETE'
        }).then(r => r.json()).then(j => {
            console.log(j);

            if (j.status.isOk)
            {
                window.location.hash = "reload";
                window.location.reload();
            }
        });
        
    }
   
}

function modifyCartQuantity(e)
{
    const button = e.target.closest("[data-cart-product-id]");
    if (!button) throw `Closest element "[data-cart-product-id]" was not found`;
    const productId = button.getAttribute("data-cart-product-id");
    const increment = button.getAttribute("data-increment");

    fetch(`/api/cart/${productId}?increment=${increment}`, {
        method: 'PATCH'
    }).then(r => r.json()).then(j => {
        console.log(j);

        if (j.status.isOk) {
            window.location.reload();
        }
        else
        {
            const errorModal = new bootstrap.Modal("#error-modal");
            errorModal.show();
        }
    });
}

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


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
        if (j.status == 201) {
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
        if (j.status == 201) // Created
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


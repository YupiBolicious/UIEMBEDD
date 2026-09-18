window.uiauth = {
    login: function (name, role, pin) {
        return fetch('/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: name, role: role, pin: pin })
        }).then(function (r) { return r.ok; });
    },
    logout: function () {
        return fetch('/logout', { method: 'POST' }).then(function (r) { return r.ok; });
    }
};
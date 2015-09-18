var _router = null;

var _user = null;

exports.set = function (router) {
    _router = router;
};

exports.get = function () {
    return _router;
};


exports.setUser = function (user) {
    _user = user;
};

exports.getUser = function () {
    return _user;
};
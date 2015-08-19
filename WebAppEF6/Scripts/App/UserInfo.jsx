var React = React || require('react');
var Reflux = Reflux || require("reflux");
var Actions = Actions || require("./Actions.js");

var AccountStore = require("./store/Account.js");


var userInfo = React.createClass({
    getInitialState: function() {
        return { currentUser: AccountStore.getCurrentUser() };
     },

    onStatusChange: function (user) {
        this.setState({
            currentUser: user
        });
    },

    componentDidMount: function () {
        this.unsubscribe = AccountStore.listen(this.onStatusChange);
    },

    componentWillUnmount: function () {
        this.unsubscribe();
    },

    logout: function () {
        Actions.doLogout();
    },

    render: function () {
        if (!this.state.currentUser || !this.state.currentUser.ID) {
            return (<div></div>);
        }else{
            return (
                 <ul className="nav navbar-nav navbar-right">
                     <li><a href="#Account/changePassword">修改密码</a></li>
                    <li><a href="#Account/login" onClick={this.logout}>退出系统</a></li>
                 </ul>
            );
        }
    }
});




if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = userInfo;
}
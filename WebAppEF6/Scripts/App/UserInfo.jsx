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
            return (<span></span>);
        }else{
            return (
                <div className="row navbar-right">
                     <ul className="col-xs-10 col-sm-10 col-md-10  nav navbar-nav ">
                         <li><a href="#Account/changePassword">修改密码</a></li>
                         <li><a href="#Account/login" onClick={this.logout}>退出系统</a></li>
                     </ul>
                     <p className="col-xs-10 col-sm-10 col-md-10  locationTitle navbar-right">山东 烟台</p>
                </div>
            );
        }
    }
});




if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = userInfo;
}
var React = require('react');

var Router = require('react-router')
  , RouteHandler = Router.RouteHandler
  , Route = Router.Route
  , DefaultRoute = Router.DefaultRoute;

var ReactBootstrap = require('react-bootstrap')
  , Nav = ReactBootstrap.Nav
  , ListGroup = ReactBootstrap.ListGroup;

var ReactRouterBootstrap = require('react-router-bootstrap')
  , NavItemLink = ReactRouterBootstrap.NavItemLink
  , ButtonLink = ReactRouterBootstrap.ButtonLink
  , ListGroupItemLink = ReactRouterBootstrap.ListGroupItemLink;


var RouterStore = require('./RouterStore');
var Actions =  require("./Actions.js");

var AccountStore = require("./store/Account.js");
//require("./store/loginStore.js");
//require("./store/registerStore.js");
var UserInfo = require("./UserInfo.jsx");
var Login = require("./login.jsx");
var Register = require("./register.jsx");
var ChangePassword = require("./changePassword.jsx");

var App = React.createClass({
    render: function () {
        return (
            <RouteHandler />
        );
    }
});

var Account = React.createClass({
    render: function () {
        return (
           <RouteHandler />
        );
    }
});

var Home = require("./home.jsx");
/*
var Home = React.createClass({
    render: function () {
       return (
          <div className="row">
            <div className="col-md-3">
                <ListGroup>
                    <ListGroupItemLink to="login">
                        Login!
                    </ListGroupItemLink>
                    <ListGroupItemLink to="destination" params={{ someparam: 'hello' }}>
                        Linky!
                    </ListGroupItemLink>
                </ListGroup>
            </div>
            <div className="col-md-9">
              <RouteHandler />
            </div>
          </div>
        );
    }
});*/

var Manage = React.createClass({

    statics: {
        willTransitionTo: function (transition, params, query, callback) {
            //console.debug("Manage willTransitionTo ", transition, params, query, callback)
            var user = AccountStore.getCurrentUser();
            if (!user || (user.Role != "Administrator" && user.Role != "ShopManager")) {
                console.debug("Manage Router  Deny", transition, user);
                transition.redirect("home");
            } else {
                callback();
            }
        },

        willTransitionFrom: function (transition, component) {
        }
    },

    render: function () {
        return (
           <div className="row">
             <div className="col-md-3">
                 <ListGroup>
                     <ListGroupItemLink to="addMember">
                         添加会员信息
                     </ListGroupItemLink>
                     <ListGroupItemLink to="updateMember">
                         修改会员信息
                     </ListGroupItemLink>
                     <ListGroupItemLink to="addMattress">
                         添加床垫信息
                     </ListGroupItemLink>
                     <ListGroupItemLink to="pointExch">
                         积分兑换
                     </ListGroupItemLink>
                     <ListGroupItemLink to="pointRule">
                         积分规则
                     </ListGroupItemLink>
                </ListGroup>
            </div>
            <div className="col-md-9">
              <RouteHandler />
            </div>
          </div>
    );
}
});

var Destination = React.createClass({
    render: function () {
        return <div>You made it!</div>;
    }
});

var AddMember = require("./addMember.jsx");
var UpdateMember = require("./updateMember.jsx");
var SellMattressTask = require("./sellMattressTask.jsx");
var PointExch = require("./pointExch.jsx");
var PointRule = require("./pointRule.jsx");

var routes = (
  <Route handler={App} path="/" name="app">
    <Route handler={Account} path="account">
      <Route name="login" path="login" handler={Login} />
      <Route name="register" path="register" handler={Register} />
      <Route name="changePassword" path="changePassword" handler={ChangePassword} />
    </Route>
    <Route handler={Home} name="home" />
    <Route handler={Manage} path="manage" name="manage">
       <Route name="addMember" path="addMember" handler={AddMember} />
       <Route name="updateMember" path="updateMember" handler={UpdateMember} />
       <Route name="addMattress" path="addMattress" handler={SellMattressTask} />
       <Route name="pointExch" path="pointExch" handler={PointExch} />
       <Route name="pointRule" path="pointRule" handler={PointRule} />
    </Route>
  </Route>
);

var AppRouter = Router.create({
    routes: routes
    //,location: Router.HistoryLocation
});

RouterStore.set(AppRouter);

React.render(<UserInfo />, document.getElementById("UserInfo"));
var user = AccountStore.getCurrentUser();

AppRouter.run(function (Handler) {
    React.render(<Handler />, document.getElementById("app_main"));
});

if (!user || !user.ID) {
    Actions.retrieveUserInfo();
}else {
    Actions.loadedUserInfo(user);
}




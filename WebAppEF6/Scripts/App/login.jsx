var React = React || require('react');
var Router = require('react-router');
var Link = Router.Link;

var Actions = Actions || require("./Actions.js");

var login = React.createClass({

    mixins: [Actions],

    handleSubmit: function (e) {
        e.preventDefault();
        var accountID = React.findDOMNode(this.refs.AccountID).value.trim();
        var accountPassword = React.findDOMNode(this.refs.Password).value.trim();

        if (!accountID || !accountPassword) {
            return;
        }

        Actions.doLogin(accountID, accountPassword, React.findDOMNode(this.refs.RememberMe).checked);
        return;
    },

    render: function () {
        return (
          <div className="row">
             <div className="col-sm-12 col-md-8">
                 <div className="row">
                      <div className="col-sm-12 col-md-10 col-lg-8  col-centered ">
                          <img className="loginBanner" src="/Images/Account_Banner_560.JPG" />
                      </div>
                 </div>
                    
             </div>

            <div className="col-md-4">
               <section>
                  <form className="form-horizontal loginModule" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>请登录系统</h4>
                        <hr />
                        <div className="text-danger"></div>
                        <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="AccountID">帐号</label>
                            <div className="col-md-9">
                                <input className="form-control" id="AccountID" ref="AccountID" type="text" autoFocus />
                                <span className="text-danger"></span>
                            </div>
                        </div>
                        <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="Password">密码</label>
                            <div className="col-md-9">
                                <input className="form-control" id="Password" ref="Password" type="password" />
                                <span asp-validation-for="Password" className="text-danger"></span>
                            </div>
                        </div>
                        <div className="form-group">
                            <div className="col-md-offset-3 col-md-9">
                                <div className="checkbox">
                                    <label>
                                      <input type="checkbox" id="RememberMe"  ref="RememberMe" />记住密码
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div className="form-group">
                            <div className="col-md-offset-3 col-md-9">
                                <button type="submit" className="btn btn-default btn-primary btn-block">登录</button>
                            </div>
                        </div>
                  </form>
               </section>
            </div>
          </div>
        );
    }
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = login;
}

var React = React || require('react');
var Router = require('react-router');
var Link = Router.Link;

var Actions = Actions || require("./Actions.js");

var register = React.createClass({

    mixins: [Actions],

    handleSubmit: function (e) {
        e.preventDefault();
        var accountID = React.findDOMNode(this.refs.AccountID).value.trim();
        var accountPassword = React.findDOMNode(this.refs.Password).value.trim();
        var confirmPassword = React.findDOMNode(this.refs.confirmPassword).value.trim();
        if (!accountID || !accountPassword || !confirmPassword) {
            return;
        } 

        Actions.register(accountID, accountPassword, confirmPassword);
        return;
    },

    render: function () {
        return (
          <div className="row">
            <div className="col-md-8">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>注册新帐号</h4>
                        <hr />
                        <div className="text-danger"></div>
                        <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="AccountID">帐号</label>
                            <div className="col-md-10">
                                <input className="form-control" id="AccountID" ref="AccountID" type="text" />
                                <span className="text-danger"></span>
                            </div>
                        </div>
                        <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="Password">密码</label>
                            <div className="col-md-10">
                                <input className="form-control" id="Password" ref="Password" type="password" />
                                <span className="text-danger"></span>
                            </div>
                        </div>
                         <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="confirmPassword">确认密码</label>
                            <div className="col-md-10">
                                <input className="form-control" id="confirmPassword" ref="confirmPassword" type="password" />
                                <span className="text-danger"></span>
                            </div>
                         </div>

    
                        <div className="form-group">
                            <div className="col-md-offset-2 col-md-10">
                                <button type="submit" className="btn btn-default">注册</button>
                            </div>
                        </div>
                        <p>
                            <Link to="login">登录已有帐号</Link>
                        </p>
                        <p>
                             <Link to="forgotPassword">忘记密码?</Link>
                        </p>
                  </form>
               </section>
            </div>
          </div>
        );
    }
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = register;
}
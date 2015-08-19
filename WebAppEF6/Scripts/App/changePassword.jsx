var React = React || require('react');
var Reflux = require("reflux");
var Actions = Actions || require("./Actions.js");

var changePassword = React.createClass({

    mixins: [Reflux.ListenerMixin],

    handleSubmit: function (e) {
        e.preventDefault();
        var OldPassw = React.findDOMNode(this.refs.OldPassw).value.trim();
        var NewPassW = React.findDOMNode(this.refs.NewPassW).value.trim();
        var NewPassWCf = React.findDOMNode(this.refs.NewPassWCf).value.trim();

        if (!OldPassw || !NewPassW || !NewPassWCf) {
            return;
        }

        if (NewPassW.length < 6) {
            alert("新密码不能低于6位！");
            return;
        }

        if (NewPassW !== NewPassWCf) {
            alert("两次输入的新密码不一样！");
            return;
        }

        Actions.changePassword(OldPassw, NewPassW, NewPassWCf);
    },

    cleanInput: function () {
        React.findDOMNode(this.refs.OldPassw).value = "";
        React.findDOMNode(this.refs.NewPassW).value = "";
        React.findDOMNode(this.refs.NewPassWCf).value = "";
    },

    onChangePasswordFail: function (errorObj) {
        if (errorObj && errorObj[0]) {
            if (errorObj[0].Code) {

                if ("PasswordMismatch" == errorObj[0].Code) {
                    alert("旧密码错误!");
                    return;
                }

                alert("错误：" + errorObj[0].Code);
            } else if (errorObj[0].ErrorMessage) {
                if ("The New password can not be same with OldPassword." == errorObj[0].ErrorMessage) {
                    alert("新旧密码不能一致!");
                }

                alert("错误：" + errorObj[0].ErrorMessage);
            }
           
        } else {
            alert("出现错误!");
        }
    },

    componentDidMount: function () {
        this.listenTo(Actions.changePasswordFail, this.onChangePasswordFail);
        //this.listenTo(Actions.changePasswordDone, this.onChangePasswordDone);
    },

    render: function () {
        return (
          <div className="row">
            <div className="col-md-8">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>修改密码</h4>
                        <hr />
                        <div className="text-danger"></div>
                        <div className="form-group">
                            <label className="col-md-2 control-label" htmlFor="OldPassw">输入旧密码：</label>
                            <div className="col-md-10">
                                <input className="form-control" id="OldPassw" ref="OldPassw" type="password" autoFocus />
                                <span className="text-danger"></span>
                            </div>
                        </div>
                      <div className="form-group">
                        <span className="col-md-offset-2 col-md-10" >请输入6位密码（数字或英文）</span>
                      </div>
                        <div className="form-group">
                            <label className="col-md-2 control-label" htmlFor="NewPassW">输入新密码：</label>
                            <div className="col-md-10">
                                <input className="form-control" id="NewPassW" ref="NewPassW" type="password" />
                                <span asp-validation-for="NewPassW" className="text-danger"></span>
                            </div>
                        </div>
                      <div className="form-group">
                            <label className="col-md-2 control-label" htmlFor="NewPassWCf">再次输入：</label>
                            <div className="col-md-10">
                                <input className="form-control" id="NewPassWCf" ref="NewPassWCf" type="password" />
                                <span asp-validation-for="NewPassWCf" className="text-danger"></span>
                            </div>
                      </div>
                        <div className="form-group">
                            <div className="col-md-offset-2 col-md-4">
                                <button type="submit" className="btn btn-default">确定</button>
                                 &nbsp;&nbsp;
                                 <button type="button" className="btn" onClick={this.cleanInput}>取消</button>
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
    module.exports = changePassword;
}
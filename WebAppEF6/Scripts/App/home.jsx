var React = React || require('react');
var Reflux = Reflux || require("reflux");
var Actions = Actions || require("./Actions.js");

var AccountStore = require("./store/Account.js");
var MemberPointDetail = require("./memberPointDetail.jsx");
var MemberRelation = require("./memberRelation.jsx");

var MemberInfo = React.createClass({

    render: function () {

        if (!this.props.user || !this.props.user.ID) {
            return (<div><h3>请先登录</h3></div>);
        }

        return (
            <div className="row">
            <div className="col-md-12">
               <section>
                   <div className="form-group">
                                <label className="col-md-2 control-label" htmlFor="MemberName">姓名：</label>
                                <div className="col-md-4">
                                    <input className="form-control" id="MemberName" ref="MemberName"
                                           type="text" readOnly
                                           value={this.props.user.Name} />
                                </div>
                        <label className="col-md-2 control-label" htmlFor="IDCard">入会时间：</label>
                        <div className="col-md-4">
                            <input className="form-control" id="IDCard" ref="IDCard"
                                   type="text" readOnly
                                   value={this.props.user.RegisterDate } />
                        </div>


                   </div>
                   <div className="form-group">
                            <label className="col-md-2 control-label" htmlFor="ExchAmount">会员类别：</label>
                              <div className="col-md-4">
                                  <input className="form-control" id="ExchAmount" ref="ExchAmount" type="text"
                                     readOnly  defaultValue={this.props.user.Level ="level1" ? "高级会员" : "普通会员" } />
                              </div>
                   </div>
               </section>
            </div>
            </div>
        );
    }
});

var Home = React.createClass({

    getInitialState: function () {
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
        return (
         <div className="row">
            <div className="col-md-12">
            <section>
                <form className="form-horizontal" method="post" role="form" >
                    <h4>基本信息</h4>
                   <MemberInfo user={this.state.currentUser} />
                   <hr />
                   <h4>积分信息</h4>
                   <MemberPointDetail MemberID={this.state.currentUser.ID} />
                   <hr />
                   <h4>线下成员</h4>
                   <MemberRelation user={this.state.currentUser} />
                   <hr />
                     <div className="form-group">
                            <div className="col-md-offset-2 col-md-4">
                                <button className="btn btn-default btn-block btn-primary" type="button"
                                         onClick={this.logout}
                                        id="submitBtn" ref="submitBtn">退出系统
                                </button>
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
    module.exports = Home;
}
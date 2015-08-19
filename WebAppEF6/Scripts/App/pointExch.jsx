var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");

var pointExch = React.createClass({

    mixins: [Reflux.ListenerMixin],

    getInitialState: function () {
        return {
            MemberPointInfo: null
        };
    },

    updateData: function (data) {
        React.findDOMNode(this.refs.ExchAmount).value = "";
        this.setState({ MemberPointInfo: data });
    },

    onMemberPointExchDone: function (data) {
        console.debug("onMemberPointExchDone", data);
        this.updateData(data);
        alert("兑换成功！");
    },

    onMemberPointExchFail: function (data) {
        console.debug("onSellMattressFail", data);
        this.updateData(data);
        var msg = "";
        if ("MemberID is not Exist." === data) {
            alert("无该会员ID信息！");
            return;
        } 
       // else if ("MattressTypeID is not Exist." === data) {
       //     msg = "床垫型号不存在！";
       // } else if ("CustomerID is not Exist." === data) {
       //     msg = "购买人ID号不存在！";
       // }

        alert("提取会员ID信息失败！" + msg);
        
    },

    onRetrieveMemberPointInfoDone: function (data) {
        console.debug("onListMattressTypeDone", data);
        React.findDOMNode(this.refs.submitBtn).disabled = false;
        this.updateData(data);
    },

    onRetrieveMemberPointInfoFail: function (data) {
        console.debug("onRetrieveMemberPointInfoFail", data);
        React.findDOMNode(this.refs.submitBtn).disabled = false;
        this.updateData(data);
        if ("MemberID is not Exist." === data) {
            alert("无该会员ID信息！");
            return;
        } 
        alert("提取会员ID信息失败！");
        
    },

    componentWillMount: function () {
        this.listenTo(Actions.memberPointExchDone, this.onMemberPointExchDone);
        this.listenTo(Actions.memberPointExchFail, this.onMemberPointExchFail);
        this.listenTo(Actions.retrieveMemberPointInfoDone, this.onRetrieveMemberPointInfoDone);
        this.listenTo(Actions.retrieveMemberPointInfoFail, this.onRetrieveMemberPointInfoFail);
    },

    onRetrieveMemberPointInfo: function (memberID) {
        console.debug("onRetrieveMemberPointInfo", memberID);
        React.findDOMNode(this.refs.submitBtn).disabled = true;
        Actions.retrieveMemberPointInfo(memberID);
    },

    handleSubmit: function (e) {
        e.preventDefault();
        var MemberID = React.findDOMNode(this.refs.MemberID).value.trim();
        var ExchAmount = React.findDOMNode(this.refs.ExchAmount).value.trim();

        if (MemberID === "") {
            alert("请输入会员ID号！");
            return;
        }
        if (!this.state.MemberPointInfo
            || this.state.MemberPointInfo.MemberID !== MemberID) {
            this.onRetrieveMemberPointInfo(MemberID);
            return;
        }

        ExchAmount = parseFloat(ExchAmount);
        if(!ExchAmount){
            alert("请输入要兑换积分数量！");
            return;
        }

        if( ExchAmount <= 0){
            alert("兑换数值必须大于0！");
            return;
        }

        if(this.state.MemberPointInfo.UsablePoint < ExchAmount ){
            alert("兑换数值不能大于可兑换积分！");
            return;
        }
        
        this.state.MemberPointInfo.ExchAmount = ExchAmount;
        Actions.memberPointExch(this.state.MemberPointInfo);
    },

    render: function () {
        return (
          <div className="row">
            <div className="col-md-12">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>积分兑换</h4>
                        <hr />
                        <div className="text-danger"></div>
                        <div className="form-group">
                            <label className="col-md-2 control-label" htmlFor="MemberID">会员ID号：</label>
                            <div className="col-md-4">
                                <input className="form-control uneditable-input" type="text" 
                                       autoFocus id="MemberID" ref="MemberID"  placeholder="输入会员ID号后按回车键" />
                        </div>
                        </div>
                        <div className="form-group"> 
                                <label className="col-md-2 control-label" htmlFor="MemberName">姓名：</label>
                                <div className="col-md-4">
                                    <input className="form-control" id="MemberName" ref="MemberName"
                                           type="text" readOnly 
                                           value={this.state.MemberPointInfo ? this.state.MemberPointInfo.MemberName : "" }/>
                                </div>
                                 <label className="col-md-2 control-label" htmlFor="IDCard">身份证号：</label>
                                 <div className="col-md-4">
                                     <input className="form-control" id="IDCard" ref="IDCard" 
                                            type="text" readOnly
                                            value={this.state.MemberPointInfo ? this.state.MemberPointInfo.IDCard : "" } />
                                 </div>

                                
                            </div>

                            <div className="form-group">
                                <label className="col-md-2 control-label" htmlFor="PointTotal">总积分：</label>
                                 <div className="col-md-4">
                                     <input className="form-control" id="PointTotal" ref="PointTotal"
                                            type="text" readOnly
                                            value={this.state.MemberPointInfo ? this.state.MemberPointInfo.PointTotal : "" } />
                                 </div>

                                  <label className="col-md-2 control-label" htmlFor="UsablePoint">可兑换积分：</label>
                                  <div className="col-md-4">
                                      <input className="form-control" id="UsablePoint" ref="UsablePoint" type="text" readOnly
                                value={this.state.MemberPointInfo ? this.state.MemberPointInfo.UsablePoint : "" } />

                        </div>
                        </div>
                        <div className="form-group">
                            <label className="col-md-2 control-label" htmlFor="ExchAmount">本次兑换积分：</label>
                              <div className="col-md-4">
                                  <input className="form-control" id="ExchAmount" ref="ExchAmount" type="text"
                                          defaultValue={this.state.MemberPointInfo ? this.state.MemberPointInfo.ExchAmount : "0" } />
                              </div>
                        </div>


                          <div className="form-group">
                              <div className="col-md-offset-2 col-md-4">
                                  <button className="btn btn-default btn-block btn-primary" type="submit" 
                                       id="submitBtn" ref="submitBtn" >兑换</button>
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
    module.exports = pointExch;
}
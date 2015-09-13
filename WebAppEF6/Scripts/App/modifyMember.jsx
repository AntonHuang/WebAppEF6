var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");

var modifyMember = React.createClass({

    mixins: [Reflux.ListenerMixin],

    getInitialState: function () {
        this.DefatulMember = {
            MemberID: "",
            Name: "",
            ReferenceID: "",
            Address: "",
            Phone: "",
            Level: "levelA",
            IDCard: ""
        };
        return { Member: this.DefatulMember };
    },

    onModifyMemberDone: function (data) {
        alert("修改成功！");
    },

    onModifyMemberFail: function () {
        alert("修改会员信息出错！");
    },

    onSelectedModifyMember: function(selectedMember){
        console.debug("onSelectedModifyMember this.isMounted()", this.isMounted(), selectedMember);

        if(!this.isMounted()){
            return;
        }

        if (selectedMember) {
            this.setState({ Member: selectedMember });
        } else {
            this.setState({ Member: this.DefatulMember });
        }
    },

    componentWillMount: function () {
        this.listenTo(Actions.modifyMemberDone, this.onModifyMemberDone);
        this.listenTo(Actions.modifyMemberFail, this.onModifyMemberFail);
        this.listenTo(Actions.selectedModifyMember, this.onSelectedModifyMember);
    },

/*
    componentDidMount: function () {
        this.AccountID = this.refs.AccountID;
        this.ReferenceID = this.refs.ReferenceID;
        this.Name = this.refs.Name;
        this.CardID = this.refs.CardID;
        this.Address = this.refs.Address;
        this.Phone = this.refs.Phone;
        this.Level = this.refs.Level;
    },*/

    componentDidUpdate: function () {
        //console.debug("componentDidUpdate", this.state.Member);
        React.findDOMNode(this.refs.AccountID).value = this.state.Member.MemberID || "";
        React.findDOMNode(this.refs.ReferenceID).value = this.state.Member.ReferenceID || "";
        React.findDOMNode(this.refs.Name).value = this.state.Member.Name || "";
        React.findDOMNode(this.refs.CardID).value = this.state.Member.IDCard || "";
        React.findDOMNode(this.refs.Address).value = this.state.Member.Address || "";
        React.findDOMNode(this.refs.Phone).value = this.state.Member.Phone || "";
        React.findDOMNode(this.refs.Level).value = this.state.Member.Level || "";
    },



    handleSubmit: function (e) {
        e.preventDefault();
        var accountID = this.state.Member.MemberID;
        // var referenceID = React.findDOMNode(this.refs.ReferenceID).value.trim();
        // var name = React.findDOMNode(this.refs.Name).value.trim();
        // var cardID = React.findDOMNode(this.refs.CardID).value.trim();
        var address = React.findDOMNode(this.refs.Address).value.trim();
        var phone = React.findDOMNode(this.refs.Phone).value.trim();
        var level = React.findDOMNode(this.refs.Level).value.trim();

        if (!accountID) {
            alert("请选择要修改的会员。");
        }

        Actions.modifyMember(accountID,  address, phone, level);
        return;
    },

    render: function () {
        return (
          <div className="row">
            <div className="col-md-12">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>修改会员信息</h4>
                        <hr />
                        <div className="text-danger"></div>
                        <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="AccountID">会员ID号：</label>
                            <div className="col-md-3">
                                <input className="form-control uneditable-input"
                                       id="AccountID" ref="AccountID"type="text" readOnly
                                       defaultValue = {this.state.Member.MemberID} />
                            </div>

                            <label className="col-md-3 control-label" htmlFor="ReferenceID">推荐人ID号：</label>
                            <div className="col-md-3">
                                <input className="form-control" id="ReferenceID" ref="ReferenceID"
                                       type="text" readOnly defaultValue={this.state.Member.ReferenceID} />

                            </div>

                        </div>

                        <div className="form-group">
                             <label className="col-md-3 control-label" htmlFor="Name">姓名：</label>
                             <div className="col-md-3">
                                 <input className="form-control" id="Name" ref="Name" type="text"
                                         readOnly defaultValue = {this.state.Member.Name} />

                             </div>

                            <label className="col-md-3 control-label" htmlFor="CardID">身份证号：</label>
                             <div className="col-md-3">
                                 <input className="form-control" id="CardID" ref="CardID" type="text"
                                        readOnly defaultValue={this.state.Member.IDCard} />

                             </div>
                        </div>


                        <div className="form-group">
                              <label className="col-md-3 control-label" htmlFor="Address">联系地址：</label>
                              <div className="col-md-3">
                                  <input className="form-control" id="Address" ref="Address" type="text"
                                           defaultValue = {this.state.Member.Address}/>

                              </div>

                             <label className="col-md-3 control-label" htmlFor="Phone">电话：</label>
                              <div className="col-md-3">
                                  <input className="form-control" id="Phone" ref="Phone" type="text"
                                         defaultValue={this.state.Member.Phone} />

                              </div>
                        </div>


                        <div className="form-group">
                              <label className="col-md-3 control-label" htmlFor="Level">会员类别：</label>
                              <div className="col-md-3">
                                  <select className="form-control" id="Level" ref="Level"  
                                          defaultValue = {this.state.Member.Level} >
                                    <option value="levelA">普通会员</option>
                                    <option value="levelB">高级会员</option>
                                    <option value="levelB1">高级会员 1级</option>
                                    <option value="levelB2">高级会员 2级</option>
                                    <option value="levelB3">高级会员 3级</option>
                                    <option value="levelB4">高级会员 4级</option>
                                    <option value="levelB5">高级会员 5级</option>
                                    <option value="levelB6">高级会员 6级</option>
                                    <option value="levelB7">高级会员 7级</option>
                                    <option value="levelB8">高级会员 8级</option>
                                  </select>
                              </div>

                              <div className="col-md-offset-3 col-md-3">
                                  <button type="submit" className="btn btn-default">保存</button>
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
    module.exports = modifyMember;
}
var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");

var addMember = React.createClass({

    mixins: [Reflux.ListenerMixin],

    onNextAccountIDDone: function(data){
        this.AccountIDDom.value = data.NextAccountID;
    },

    onNextAccountIDFail: function () {
        alert("无法生成新的会员ID。");
    },

    componentWillMount: function(){
        this.listenTo(Actions.nextAccountIDDone,this.onNextAccountIDDone);
        this.listenTo(Actions.nextAccountIDFail,this.onNextAccountIDFail);

        Actions.nextAccountID();
    },

    componentDidMount: function () {
        this.AccountIDDom = React.findDOMNode(this.refs.AccountID);
        this.ReferenceIDDom = React.findDOMNode(this.refs.ReferenceID);
        this.NameDom = React.findDOMNode(this.refs.Name);
        this.CardIDDom = React.findDOMNode(this.refs.CardID);
        this.AddressDom = React.findDOMNode(this.refs.Address);
        this.PhoneDom = React.findDOMNode(this.refs.Phone);
        this.LevelDom = React.findDOMNode(this.refs.Level);
    },

    handleSubmit: function (e) {
        e.preventDefault();
        var accountID = this.AccountIDDom.value.trim();
        var referenceID = this.ReferenceIDDom.value.trim();
        var name = this.NameDom.value.trim();
        var cardID = this.CardIDDom.value.trim();
        var address = this.AddressDom.value.trim();
        var phone = this.PhoneDom.value.trim();
        var level = this.LevelDom.value.trim();
        if (!accountID) {
            alert("会员ID不能留空！");
            return;
        } 

        Actions.register(accountID, referenceID, name, cardID, address, phone, level);
        return;
    },

    render: function () {
        return (
          <div className="row">
            <div className="col-md-12">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>注册会员信息</h4>
                        <hr />
                        <div className="text-danger"></div>
                        <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="AccountID">会员ID号：</label>
                            <div className="col-md-3">
                                <input className="form-control uneditable-input" 
                                       id="AccountID" ref="AccountID" 
                                       type="text" readOnly />
                                
                            </div>

                            <label className="col-md-3 control-label" htmlFor="ReferenceID">推荐人ID号：</label>
                            <div className="col-md-3">
                                <input className="form-control" id="ReferenceID" ref="ReferenceID"
                                       type="text" autoFocus />

                            </div>

                        </div>
                       <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="Name">姓名：</label>
                            <div className="col-md-3">
                                <input className="form-control" id="Name" ref="Name" type="text" />
                                
                            </div>

                           <label className="col-md-3 control-label" htmlFor="CardID">身份证号：</label>
                            <div className="col-md-3">
                                <input className="form-control" id="CardID" ref="CardID" type="text" />

                            </div>
                       </div>

                      <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="Address">联系地址：</label>
                            <div className="col-md-3">
                                <input className="form-control" id="Address" ref="Address" type="text" />
                                
                            </div>

                          <label className="col-md-3 control-label" htmlFor="Phone">电话：</label>
                            <div className="col-md-3">
                                <input className="form-control" id="Phone" ref="Phone" type="text" />

                            </div>
                      </div>


                      <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="Level">会员类别：</label>
                            <div className="col-md-3">
                                <select className="form-control" id="Level" ref="Level" defaultValue = "levelA" >
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
                      </div>

    
                        <div className="form-group">
                            <div className="col-md-offset-3 col-md-9">
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
    module.exports = addMember;
}
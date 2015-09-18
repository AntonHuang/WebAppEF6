var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");
var RouterStore = require('./RouterStore');


var SaleMemberPoint = require("./saleMemberPoint.jsx");

var SellMattressResult = React.createClass({

    getDefaultProps: function () {
        return {
            sellMattressData: {
                MattressID: "",
                MattressTypeName: "",
                DeliveryAddress: "",
                CustomerID: "",
                SaleDate: "",
                Gifts: ""
            },
            MemberPointItems: {
                MemberName: "",
                MemberID: "",
                PointCount: "",
                Up1Name: "",
                Up1ID: "",
                Up1PointCount: "",
                Up2Name: "",
                Up2ID: "",
                Up2PointCount: ""
            }
        };
    },

    componentDidUpdate: function (newProps, newState) {
        React.findDOMNode(this.refs.MattressID).value = this.props.sellMattressData.MattressID;
        React.findDOMNode(this.refs.MattressTypeName).value = this.props.sellMattressData.MattressTypeName;
        React.findDOMNode(this.refs.DeliveryAddress).value = this.props.sellMattressData.DeliveryAddress;
        React.findDOMNode(this.refs.CustomerID).value = this.props.sellMattressData.CustomerID;
        React.findDOMNode(this.refs.SaleDate).value = this.props.sellMattressData.SaleDate;
        React.findDOMNode(this.refs.Gifts).value = this.props.sellMattressData.Gifts;
    },

    returnToManage: function () {
        RouterStore.get().transitionTo("manage");
    },

    render: function () {
        return (
          <div className="row">
            <div className="col-md-12">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>积分添加结果</h4>
                        <hr />
                        <div className="text-danger"></div>
                        <div className="form-group">
                            <label className="col-md-2 control-label" htmlFor="MattressID">床垫编号：</label>
                            <div className="col-md-4">
                                <input className="form-control uneditable-input" type="text"
                                       readOnly defaultValue={this.props.sellMattressData.MattressID}
                                       id="MattressID" ref="MattressID" />
                            </div>

                    <label className="col-md-2 control-label" htmlFor="MattressTypeID">床垫型号：</label>
                    <div className="col-md-4">
                        <input className="form-control  uneditable-input" id="MattressTypeName" ref="MattressTypeName"
                               readOnly defaultValue={this.props.sellMattressData.MattressTypeName } />
                    </div>

                        </div>
                    <div className="form-group">
                             <label className="col-md-2 control-label" htmlFor="DeliveryAddress">送货地址：</label>
                             <div className="col-md-4">
                                 <input className="form-control  uneditable-input" 
                                        id="DeliveryAddress" ref="DeliveryAddress" type="text" readOnly
                                        defaultValue={this.props.sellMattressData.DeliveryAddress } />
                             </div>

                            <label className="col-md-2 control-label " htmlFor="CustomerID">购买人ID号：</label>
                             <div className="col-md-4">
                                 <input className="form-control  uneditable-input" id="CustomerID" ref="CustomerID" type="text" 
                                        readOnly
                                        defaultValue={this.props.sellMattressData.CustomerID } />

                             </div>
                    </div>

    <div className="form-group">
          <label className="col-md-2 control-label" htmlFor="SaleDate">购买时间：</label>
          <div className="col-md-4">
              <input className="form-control uneditable-input" id="SaleDate" ref="SaleDate" type="date"
                   readOnly   defaultValue={this.props.sellMattressData.SaleDate } />

          </div>

        <label className="col-md-2 control-label" htmlFor="IsUseCashCoupon">使用代金券：</label>
                                <div className="col-md-4">

                                      <input className="form-control uneditable-input" id="IsUseCashCoupon" ref="IsUseCashCoupon" type="text"
                                             readOnly defaultValue={this.props.sellMattressData.IsUseCashCoupon==1 ? "是" : "否" } />
                                </div>

    </div>
<div className="form-group">
    <label className="col-md-2 control-label" htmlFor="Gifts">赠送礼品：</label>
      <div className="col-md-4">
          <textarea className="form-control  uneditable-input" id="Gifts" ref="Gifts" rows="3"
                readOnly    defaultValue={this.props.sellMattressData.Gifts } />
      </div>
</div>

                      <hr />
                      <SaleMemberPoint MemberPointItems={this.props.MemberPointItems} />
                      <hr />


  <div className="form-group">
      <div className="col-md-offset-2 col-md-4">
          <button type="submit" className="btn btn-default btn-block" onClick={this.returnToManage}>确定</button>
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
    module.exports = SellMattressResult;
}
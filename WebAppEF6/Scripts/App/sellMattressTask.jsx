var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");

var SellMattress = require("./sellMattress.jsx");
var SellMattressResult = require("./sellMattressResult.jsx");

var sellMattressTask = React.createClass({

    mixins: [Reflux.ListenerMixin],

    getInitialState: function () {
        return {
            sellMattressNo: "",
            showResult: false,
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

    onSellMattressDone: function (data) {
        console.debug("onSellMattressDone", data);
        alert("添加成功！");

        this.setState({
            sellMattressNo: data.saleToCustomerID,
            showResult: data.saleToCustomerID !== "",
            sellMattressData: data.sellMattressData,
            MemberPointItems: data.memberPointItems,
        });
    },

    componentWillUpdate: function (newProps, newState) {
        console.debug("componentWillUpdate", this.state);
    },

    componentDidUpdate: function (newProps, newState) {
        console.debug("componentDidUpdate", this.state);
    },
     
    componentWillMount: function () {
        this.listenTo(Actions.sellMattressDone, this.onSellMattressDone);
    },

    render: function () {

        function step(state) {
            if (state.showResult) {
                return <SellMattressResult sellMattressData={state.sellMattressData}
                            MemberPointItems={state.MemberPointItems} />;
            } else {
                return <SellMattress />
            }

        }

        return (
            <div className="row">
                <div className="col-md-12">
                   {step(this.state)}
                </div>
            </div>
        );
    }

});



if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = sellMattressTask;
}
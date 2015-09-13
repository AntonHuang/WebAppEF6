var React = React || require('react');

var SaleMemberPointItem = require("./saleMemberPointItem.jsx");

var SaleMemberPoint = React.createClass({

    getInitialState: function () {
        return {
            SelfMP: {
                Name: "",
                MemberID: "",
                Point: ""
            },
            Up1MP: {
                Name: "",
                MemberID: "",
                Point: ""
            },
            Up2MP: {
                Name: "",
                MemberID: "",
                Point: ""
            }
        };
    },

    getDefaultProps: function () {
        return {
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

    render: function () {
        return (
    <div className="row form-inline">
        <div className="col-md-12">
            <section>
                <hr />
                <h4>积分信息</h4>
                 <div className="row">
                    <h5>推荐人积分信息：</h5>
                    <div className="col-md-12">
                        <SaleMemberPointItem MemberPoint={{
                Name: this.props.MemberPointItems.MemberName || "",
                MemberID: this.props.MemberPointItems.MemberID || "",
                Point: this.props.MemberPointItems.PointCount,
            }} />
                    </div>
                 </div>
                <div className="row">
                    <h5>上一级积分信息：</h5>
                    <div className="col-md-12">
                        <SaleMemberPointItem MemberPoint={{
                Name: this.props.MemberPointItems.Up1Name || "",
                MemberID: this.props.MemberPointItems.Up1ID || "",
                Point: this.props.MemberPointItems.Up1PointCount,
            }} />
                    </div>
                </div>
                <div className="row">
                    <h5>上两级积分信息：</h5>
                    <div className="col-md-12 ">
                        <SaleMemberPointItem MemberPoint={{
                Name: this.props.MemberPointItems.Up2Name || "",
                MemberID: this.props.MemberPointItems.Up2ID || "",
                Point: this.props.MemberPointItems.Up2PointCount,
            }} />
                    </div>
                </div>
            </section>
        </div>
    </div>);

    }
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = SaleMemberPoint;
}
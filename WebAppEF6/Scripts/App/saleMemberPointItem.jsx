var React = React || require('react');

var SaleMemberPointItem = React.createClass({




    render: function () {
        return (
            <div className="row form-inline">
            <div className="col-md-12">
               <label className="col-md-2 control-label" htmlFor="Name">姓名：</label>
                <div className="col-md-2">
                     <input className="form-control uneditable-input" type="text"
                        id="Name" ref="Name" readOnly value={this.props.MemberPoint.Name} />
                </div>

                <label className="col-md-2 control-label" htmlFor="MemberID">会员ID号：</label>
                <div className="col-md-2">
                     <input className="form-control uneditable-input" type="text"
                        id="MemberID" ref="MemberID" readOnly value={this.props.MemberPoint.MemberID} />
                </div>
                <label className="col-md-2 control-label" htmlFor="Point">积分：</label>
                <div className="col-md-2">
                     <input className="form-control uneditable-input" type="text"
                        id="Point" ref="Point" readOnly value={this.props.MemberPoint.Point} />
                </div>
            </div>
        </div>);
    }
});

if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = SaleMemberPointItem;
}
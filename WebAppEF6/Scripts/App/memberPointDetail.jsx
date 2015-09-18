var React = React || require('react');
var Reflux = Reflux || require("reflux");
var Actions = Actions || require("./Actions.js");

var AccountStore = require("./store/Account.js");
var GriddleWithCallback = require("./griddleWithCallback.jsx");

var tableNoDataMessage = "--没找到积分记录--";
var tableColumns = ["ProductBuyerName", "ProductTypeName", "BuyerRelation", "DealDate", "Point", "CurrentTotalPoint"];
var tablecolumsMeteData = [
 {
     "columnName": "ProductBuyerName",
     "order": 1,
     "locked": false,
     "visible": true,
     "displayName": "购买人"
 },
  {
      "columnName": "ProductTypeName",
      "order": 2,
      "locked": false,
      "visible": true,
      "displayName": "床垫型号"
  },
  {
      "columnName": "BuyerRelation",
      "order": 3,
      "locked": false,
      "visible": true,
      "displayName": "关系"
  },
  {
      "columnName": "DealDate",
      "order": 4,
      "locked": false,
      "visible": true,
      "displayName": "购买时间"
  }, {
      "columnName": "Point",
      "order": 4,
      "locked": false,
      "visible": true,
      "displayName": "增加积分"
  }, {
      "columnName": "CurrentTotalPoint",
      "order": 4,
      "locked": false,
      "visible": true,
      "displayName": "总积分"
  }, ]


var MemberPointDetail = React.createClass({

    mixins: [Reflux.ListenerMixin],

    getDefaultProps: function () {
        return {
            pointItemTableUpdateID: "MemberPointDetail" + Math.random(),
        };
    },

    getInitialState: function () {
        return {
            MemberPointInfo: null,
            totalResults: 0,
            tableCallback: null
        };
    },

    loadData: function (filterString, sortColumn, sortAscending, page, pageSize, callback) {

        if (this.props.MemberID) {
            this.state.tableCallback = callback;
            this.state.pageSize = pageSize;
            Actions.findMemberPointDetail(this.props.MemberID, page, pageSize, this.state.lastFilterData);
        } else {
            callback({
                results: []
            });
        }
    },

    onFindMemberPointDetailDone: function (data) {

      
        var maxPage = Math.ceil(data.TotalSize / this.state.pageSize)
        var page = 0;
        // If the current page is larger than the max page, reset the page.
        if (page >= maxPage) {
            page = maxPage - 1;
        }

        var tableData = {
            results: data.MemberPointItems,
            totalResults: data.TotalSize,
            pageSize: this.state.pageSize
        };

        this.state.totalResults = data.TotalSize;

        if (typeof (this.state.tableCallback) === 'function') {
            this.state.tableCallback(tableData);
        } else {
            tableData.page = page;
            tableData.maxPage = maxPage;
            Actions.updateTableData(this.props.pointItemTableUpdateID, tableData);
        }
    },

    onFindMemberPointDetailFail: function (data) {
        console.debug("onFindMemberPointDetailFail", data);
        //alert("查询出错！");
        if (typeof (this.state.tableCallback) === 'function') {
            this.state.tableCallback({
                results: []
            });
        }
       
    },

    onRetrieveMemberPointInfoDone: function (data) {
        this.setState({ MemberPointInfo: data });
    },

    onRetrieveMemberPointInfoFail: function (data) {
        console.debug("onRetrieveMemberPointInfoFail", data);
        //alert("查询出错！");
    },

    componentWillMount: function () {
        this.listenTo(Actions.findMemberPointDetailDone, this.onFindMemberPointDetailDone);
        this.listenTo(Actions.findMemberPointDetailFail, this.onFindMemberPointDetailFail);
        this.listenTo(Actions.retrieveMemberPointInfoDone, this.onRetrieveMemberPointInfoDone);
        this.listenTo(Actions.retrieveMemberPointInfoFail, this.onRetrieveMemberPointInfoFail);
    },

    componentDidMount: function () {
        if (this.props.MemberID) {
            Actions.retrieveMemberPointInfo(this.props.MemberID);
        }
    },

    render: function () {
        return (
            <div className="row">
            <div className="col-md-12">
            <div className="form-group">
                <label className="col-md-2 control-label" htmlFor="PointTotal">总积分：</label>
                 <div className="col-md-4">
                     <input className="form-control" id="PointTotal" ref="PointTotal"
                            type="text" readOnly
                            value={this.state.MemberPointInfo ? this.state.MemberPointInfo.PointTotal: "" } />
                 </div>

                <label className="col-md-2 control-label" htmlFor="UsablePoint">可兑换积分：</label>
                <div className="col-md-4">
                    <input className="form-control" id="UsablePoint" ref="UsablePoint" type="text" readOnly
                           value={this.state.MemberPointInfo ? this.state.MemberPointInfo.UsablePoint : "" } />

                </div>
            </div>
                <div className="form-group">
                 <div className="col-md-12">
                      <GriddleWithCallback showFilter={false} showSettings={false} enableSort={false}
                                           resultsPerPage = { 10 }
                                           tableClassName="table-hover table table-striped"
                                           noDataMessage={tableNoDataMessage}
                                           columns={tableColumns}
                                           columnMetadata={tablecolumsMeteData}
                                           getExternalResults={this.loadData}
                                           tableUpdateID={this.props.pointItemTableUpdateID}
                                           nextText="下一页"
                                           previousText="上一页" />
                 </div>
                </div>
            </div>
            </div>
        );
    }
});

if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = MemberPointDetail;
}
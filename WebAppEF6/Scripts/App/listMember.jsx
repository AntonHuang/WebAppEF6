var React = React || require('react');
var Griddle = require('griddle-react');
var GriddleWithCallback = require("./griddleWithCallback.jsx");
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");


var tableNoDataMessage = "没找到匹配的结果，请选择合适的查询条件。";
var tableColumns = ["MemberID", "Name", "Phone", "ReferenceID"];
var tablecolumsMeteData = [
 {
     "columnName": "MemberID",
     "order": 1,
     "locked": false,
     "visible": true,
     "displayName": "会员ID"
 },
  {
      "columnName": "Name",
      "order": 2,
      "locked": false,
      "visible": true,
      "displayName": "姓名"
  },
  {
      "columnName": "Phone",
      "order": 3,
      "locked": false,
      "visible": true,
      "displayName": "电话"
  },
  {
      "columnName": "ReferenceID",
      "order": 4,
      "locked": false,
      "visible": true,
      "displayName": "推荐人会员ID"
  }, ];


var listMember = React.createClass({
    
    mixins: [Reflux.ListenerMixin],

    getDefaultProps: function () {
        return {
            memberTableUpdateID: "listMember" + Math.random()
        }
    },

    getInitialState: function () {
        return {
            totalResults: 0,
            tableCallback: null
        };
    },

    loadData: function (filterString, sortColumn, sortAscending, page, pageSize, callback) {


        if (this.state.totalResults > 0) {
            this.state.tableCallback = callback;
            this.retrieveMembers(page, pageSize, this.state.lastFilterData);
        } else {
            callback({
                results: [],
                pageSize: this.pageSize
            });
        }


    },

    onFindMemberDone: function (data) {

        var pageSize = 10;
        var maxPage = Math.ceil(data.TotalSize / pageSize)
        var page = 0;
        // If the current page is larger than the max page, reset the page.
        if (page >= maxPage) {
            page = maxPage - 1;
        }

        var tableData = {
            results: data.Members,
            totalResults: data.TotalSize,
            pageSize: pageSize
        };

        this.state.totalResults = data.TotalSize;

        if (typeof (this.state.tableCallback) === 'function') {
            this.state.tableCallback(tableData);
        } else {
            tableData.page = page;
            tableData.maxPage = maxPage;
            Actions.updateTableData(this.props.memberTableUpdateID, tableData);
        }
    },

    onFindMemberFail: function () {
        alert("查询出错！");
    },

    onModifyMemberDone: function (data) {

        Actions.refreshTableCuurentPage(this.props.memberTableUpdateID);
    },

    componentWillMount: function () {
        this.listenTo(Actions.findMemberDone,this.onFindMemberDone);
        this.listenTo(Actions.findMemberFail,this.onFindMemberFail);
        this.listenTo(Actions.modifyMemberDone, this.onModifyMemberDone);

        this.CurrentPage = 0;
        this.pageSize = 10;
    },


    onTableRowClick: function (rowD, e) {
        Actions.selectedModifyMember(rowD.props.data);
    },

    retrieveMembers: function (pageIdx, pageSize, filterData) {

        if (!filterData){
            this.state.lastFilterData = {
                accountID: React.findDOMNode(this.refs.Filter).value.trim(),
                referenceID: "",
                name: "",
                cardID: "",
                phone: ""
            };
            filterData = this.state.lastFilterData;
        }

        Actions.findMember(filterData.accountID, filterData.referenceID, filterData.name,
              filterData.cardID, filterData.phone, pageIdx, pageSize);

    },

    handleSubmit: function (e) {
        e.preventDefault();
        this.state.totalResults = 0;
        this.state.tableCallback = null;
        Actions.selectedModifyMember(null);
        this.retrieveMembers(this.CurrentPage, this.pageSize);

    },

    render: function () {
        return (
          <div className="row">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>查询会员信息</h4>
                        <hr />
                        <div className="form-group">
                            <div className="col-md-4">
                                <input className="form-control uneditable-input" autoFocus placeholder="姓名,电话,身份证号,推荐人ID号,会员ID号"
                                       id="Filter" ref="Filter" type="text" />
                            </div>
                           <div className="col-md-2">
                                <button type="submit" className="btn btn-default btn-block btn-primary">查找</button>
                            </div>
                        </div>
                  </form>
               </section>

             <GriddleWithCallback showFilter={false} showSettings={false} enableSort={false}
                                  tableClassName= "table-hover table table-striped"
                                  noDataMessage={tableNoDataMessage}
                                  columns={tableColumns}
                                  columnMetadata={tablecolumsMeteData}
                                  getExternalResults={this.loadData} ref="memberDataTable"
                                  onRowClick={this.onTableRowClick}
                                  tableUpdateID = {this.props.memberTableUpdateID }
                                  nextText="下一页"
                                  previousText="上一页" />
          </div>
        );
    }
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = listMember;
}
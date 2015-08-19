var React = React || require('react');
var Reflux = Reflux || require("reflux");
var Actions = Actions || require("./Actions.js");

var ReactBootstrap = require('react-bootstrap')
var PanelGroup = ReactBootstrap.PanelGroup;
var Panel = ReactBootstrap.Panel;
var ButtonGroup = ReactBootstrap.ButtonGroup;
var Button = ReactBootstrap.Button;

var AccountStore = require("./store/Account.js");


var MemberRelation = React.createClass({

    mixins: [Reflux.ListenerMixin],

    getInitialState: function () {
        return {
            RelationItems: []
        };
    },

    onRetrieveMemberRelationInfoDone: function (data) {
        console.debug("onRetrieveMemberRelationInfoDone", data);

        //if (data) {
       //     data = data.map(this.transData);
       // }
       // concole.debug("onRetrieveMemberRelationInfoDone transData", data);
        this.setState({ RelationItems: data });
    },

    transData: function(item){
        var node = {
            text: item.ChildName
        };

        if (item.children) {
            node.nodes = item.children.map(this.transData);
        }
        return node;
    },

    onRetrieveMemberRelationInfoFail: function (data) {
        console.debug("onRetrieveMemberRelationInfoFail", data);
        this.setState({ RelationItems : []});
    },

    componentWillMount: function () {
        this.listenTo(Actions.retrieveMemberRelationInfoDone, this.onRetrieveMemberRelationInfoDone);
        this.listenTo(Actions.retrieveMemberRelationInfoFail, this.onRetrieveMemberRelationInfoFail);
        
    },

    componentDidMount: function () {
        Actions.retrieveMemberRelationInfo(this.props.user.ID);
    },

    render: function () {
      
        if (!this.props.user || !this.props.user.ID) {
            return (<p> -- 无 -- </p>);
        }

        function showChild(item) {
            return <Button>{item.ChildName}</Button>;
        }

        function checkData(data) {
            if(!data || data.length == 0){
                return <p> -- 无 -- </p>;
            }
        }

        return (
           <PanelGroup  accordion>
               {checkData(this.state.RelationItems)}
               {this.state.RelationItems.map(function(item,idx){
                    if(item.children && item.children.length > 0){
               return (<Panel header={item.ChildName} eventKey={idx+1}>
                        <ButtonGroup>{item.children? item.children.map(showChild) : "" }</ButtonGroup>
                       </Panel>);
                    }else{
                        return (<Panel header={item.ChildName} eventKey={idx+1} />);
                    }
                    
               })}
          </PanelGroup>
        );
    }

});
if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = MemberRelation;
}
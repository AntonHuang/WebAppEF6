var React = React || require('react');
//var Actions = Actions || require("./Actions.js");
//var Reflux = Reflux || require("reflux");

var ListMember = require("./listMember.jsx");
var ModifyMember = require("./modifyMember.jsx");


var updateMember = React.createClass({
    render: function () {
        return (
            <div>
                <ListMember />
                <hr />
                <ModifyMember />
            </div>
        );
    }

});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = updateMember;
}

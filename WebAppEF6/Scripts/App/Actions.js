var Reflux = require("reflux");

var Actions = Reflux.createActions([
    "retrieveUserInfo",
    "doLogin",
    "doLogout",
    "loadedUserInfo",

    "changePassword",
    "changePasswordDone",
    "changePasswordFail",

    "nextAccountID",
    "nextAccountIDDone",
    "nextAccountIDFail",

    "modifyMember",
    "modifyMemberDone",
    "modifyMemberFail",

    "register",
    "registerDone",
    "registerFail",

    "findMember",
    "findMemberDone",
    "findMemberFail",

    "listMattressType",
    "listMattressTypeDone",
    "listMattressTypeFail",

    "sellMattress",
    "sellMattressDone",
    "sellMattressFail",

    "getPiontRule",
    "getPiontRuleDone",
    "getPiontRuleFail",

    "modifyPiontRule",
    "modifyPiontRuleDone",
    "modifyPiontRuleFail",

    "retrieveMemberPointInfo",
    "retrieveMemberPointInfoDone",
    "retrieveMemberPointInfoFail",

    "memberPointExch",
    "memberPointExchDone",
    "memberPointExchFail",

    "findMemberPointDetail",
    "findMemberPointDetailDone",
    "findMemberPointDetailFail",

    "retrieveMemberRelationInfo",
    "retrieveMemberRelationInfoDone",
    "retrieveMemberRelationInfoFail",

     "updateTableData",
     "refreshTableCuurentPage",

     "selectedModifyMember",
     
]);

if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = Actions;
}
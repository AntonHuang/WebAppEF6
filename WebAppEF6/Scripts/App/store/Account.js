﻿var Reflux = require("reflux");
var $ = require("jquery");
require("../jquerytoken.js");
var Actions = require("../Actions.js");
var RouterStore = require('../RouterStore');

$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    console.debug("errorThrown", thrownError);
    console.debug("responseText", jqxhr.responseText);
    console.debug("event", event)
    
    if (jqxhr.status === 401) {
        console.debug("401 status. Redirect to login page.");
        RouterStore.get().transitionTo("login");
    }
});

var User = User || {};

var accountStore = Reflux.createStore({
    
    init: function () {
        this.listenTo(Actions.retrieveUserInfo, this.getUserInfo);
        this.listenTo(Actions.doLogin, this.login);
        this.listenTo(Actions.doLogout, this.logout);
        this.listenTo(Actions.register, this.register);
        this.listenTo(Actions.loadedUserInfo, this.loadedUserInfo);
        this.listenTo(Actions.changePassword, this.changePassword);

        this.listenTo(Actions.nextAccountID, this.onNextAccountID);

        this.listenTo(Actions.findMember, this.onFindMember);
        this.listenTo(Actions.modifyMember, this.onModifyMember);

        this.listenTo(Actions.sellMattress, this.onSellMattress);
        this.listenTo(Actions.listMattressType, this.onListMattressType);

        this.listenTo(Actions.getPiontRule, this.onGetPiontRule);
        this.listenTo(Actions.modifyPiontRule, this.onModifyPiontRule);

        this.listenTo(Actions.retrieveMemberPointInfo, this.onRetrieveMemberPointInfo);
        this.listenTo(Actions.memberPointExch, this.onMemberPointExch);
        
        this.listenTo(Actions.findMemberPointDetail, this.onFindMemberPointDetail);

        this.listenTo(Actions.retrieveMemberRelationInfo, this.onRetrieveMemberRelationInfo);

    },

    getCurrentUser: function () {
        console.debug("getCurrentUser", User);
        return User;
    },

    loadedUserInfo: function (user) {
        var currUser = user || this.getCurrentUser();
        if (currUser && currUser.ID) {
            if (currUser.NeedToChangePassword) {
                RouterStore.get().transitionTo("changePassword");
            } else if (currUser.Role === "Administrator" || currUser.Role === "ShopManager") {
                RouterStore.get().transitionTo("manage");
            } else {
                RouterStore.get().transitionTo("home");
            }
        } else {
            RouterStore.get().transitionTo("login");
        }
        accountStore.trigger(currUser);
    },

    getUserInfo: function () {
        console.debug("do getUserInfo");
        $.ajax({
            type: "GET",
            url: "/Account/UserInfo"
        }).success(function (data) {
            console.debug("getUserInfo done!");
            User = data;
            Actions.loadedUserInfo(User);
            //accountStore.trigger(User);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            if (jqxhr.status === 404) {
                User = {};
                Actions.loadedUserInfo(User);
            }
        });
    },

    login: function (userName, userPassword, remember){
        console.debug("do login");
        var requestData = { UserID: userName, Password: userPassword, RememberMe: remember };
        $.ajaxAntiForgery({
            type: "POST",
            url: "/Account/Login",
            data: requestData,
            dataType: "json"
        }).success(function (data,  textStatus, jqXHR) {
            console.debug("login done!");
            User = data;
            //console.debug("response = " + jqXHR.responseText);
            Actions.loadedUserInfo(User);
            //accountStore.trigger(User);
            //RouterStore.get().transitionTo("/");
        }).fail(function (jqxhr, textStatus, errorThrown) {
            console.debug("login error!");
            alert("用户名或者密码错误！");
        });
    },

    logout: function (userName, userPassword, remember) {
        console.debug("do logout");
        $.ajaxAntiForgery({
            type: "POST",
            url: "/Account/LogOff"
        }).success(function (data, textStatus, jqXHR) {
            console.debug("logout done!");
            User = {};
            Actions.loadedUserInfo(User);
        });
    },


    register: function (accountID, referenceID, name, cardID, address, phone, level) {
        var requestData = {
            AccountID: accountID,
            ReferenceID: referenceID,
            Name: name,
            cardID: cardID,
            Address: address,
            Phone: phone,
            Level: level
        };
        $.ajaxAntiForgery({
            type: "POST",
            url: "/Account/Register",
            data: requestData,
            dataType: "json"
        }).done(function (data) {
            console.debug("register done!");
            alert("添加成功！");
            RouterStore.get().transitionTo("manage");
            Actions.registerDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            if (/application\/json/.test(jqxhr.getResponseHeader('Content-Type'))) {
                try {
                    jqxhr.appError = $.parseJSON(jqxhr.responseText);
                } catch (e) {
                    console.debug(e);
                }
            }
            if ("ReferenceID is not exist." === jqxhr.appError) {
                alert("推荐人ID号不存在！")
            }
            Actions.registerFail(jqxhr.appError);
        });
    },

    changePassword: function (oldPassw, newPassW, newPassWCf) {
        console.debug("do changePassword");
        var requestData = { OldPassword: oldPassw, NewPassword: newPassW, ConfirmPassword: newPassWCf };
        $.ajaxAntiForgery({
            type: "POST",
            url: "/Account/ChangePassword",
            data: requestData,
            dataType: "json"
        }).done(function (data, textStatus, jqXHR) {
            console.debug("changePassword done!");
            alert("修改成功！");
            var currUser = accountStore.getCurrentUser();
            currUser.NeedToChangePassword = false;
            Actions.loadedUserInfo();
        }).fail(function (jqxhr, textStatus, errorThrown) {
            if (/application\/json/.test(jqxhr.getResponseHeader('Content-Type'))) {
                try {
                    jqxhr.appError = $.parseJSON(jqxhr.responseText);
                } catch (e) {
                    console.debug(e);
                }
            }
            Actions.changePasswordFail(jqxhr.appError);
        });
    },


    onNextAccountID: function(){
        console.debug("do NextAccountID");
        $.ajax({
            type: "GET",
            url: "/Account/NextAccountID",
            dataType: "json"
        }).done(function (data) {
            console.debug("NextAccountID done!");
            Actions.nextAccountIDDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            if (/application\/json/.test(jqxhr.getResponseHeader('Content-Type'))) {
                try {
                    jqxhr.appError = $.parseJSON(jqxhr.responseText);
                } catch (e) {
                    console.debug(e);
                }
            }
            Actions.nextAccountIDFail(jqxhr.appError);
        });
    
    },


    onFindMember: function(accountID, referenceID, name, cardID, phone, page, pageSize){
        console.debug("do onFindMember");
        var requestData = {
            MemberID: accountID,
            ReferenceID: referenceID,
            Name: name,
            IDCard: cardID,
            Phone: phone,
            Page: page,
            PageSize: pageSize
        };
        $.ajax({
            type: "GET",
            url: "/Account/findMember",
            data: requestData,
            dataType: "json"
        }).done(function (data) {
            console.debug("onFindMember done!", data);
            Actions.findMemberDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            if (/application\/json/.test(jqxhr.getResponseHeader('Content-Type'))) {
                try {
                    jqxhr.appError = $.parseJSON(jqxhr.responseText);
                } catch (e) {
                    console.debug(e);
                }
            }
            Actions.findMemberFail(jqxhr.appError);
        });


    },

    onModifyMember: function (accountID, address, phone, level) {
        console.debug("do onModifyMember");
        var requestData = {
            MemberID: accountID,
            Phone: phone,
            Level: level,
            Address: address
        };
        $.ajax({
            type: "POST",
            url: "/Account/ModifyMember",
            data: requestData,
            dataType: "json"
        }).done(function (data) {
            console.debug("onModifyMember done!", data);
            Actions.modifyMemberDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            if (/application\/json/.test(jqxhr.getResponseHeader('Content-Type'))) {
                try {
                    jqxhr.appError = $.parseJSON(jqxhr.responseText);
                } catch (e) {
                    console.debug(e);
                }
            }
            Actions.modifyMemberFail(jqxhr.appError);
        });

    },

    onListMattressType: function () {
        console.debug("do onlistMattressType");
        $.ajax({
            type: "GET",
            url: "/Mattress/listMattressType",
            dataType: "json"
        }).done(function (data) {
            console.debug("onlistMattressType done!");
            Actions.listMattressTypeDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            if (/application\/json/.test(jqxhr.getResponseHeader('Content-Type'))) {
                try {
                    jqxhr.appError = $.parseJSON(jqxhr.responseText);
                } catch (e) {
                    console.debug(e);
                }
            }
            Actions.listMattressTypeFail(jqxhr.appError);
        });
    },

    onSellMattress: function (MattressID, MattressTypeID, DeliveryAddress, CustomerID, SaleDate, Gifts, IsUseCashCoupon) {
        console.debug("do onSellMattress");
        var self = this;
        var requestData = {
            MattressID: MattressID, 
            MattressTypeID: MattressTypeID, 
            DeliveryAddress: DeliveryAddress, 
            CustomerID: CustomerID,
            SaleDate: SaleDate,
            Gifts: Gifts,
            IsUseCashCoupon: IsUseCashCoupon
        };
        $.ajax({
            type: "POST",
            url: "/Mattress/Sell",
            data: requestData,
            dataType: "json"
        }).done(function (data) {
            console.debug("onSellMattress done!", data);
            Actions.sellMattressDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            self.parseErrorJson(jqxhr);
            Actions.sellMattressFail(jqxhr.appError);
        });

    },

    onGetPiontRule: function () {
        console.debug("do onGetPiontRule");
        var self = this;
        $.ajax({
            type: "GET",
            url: "/Setting/PointRule",
            dataType: "json"
        }).done(function (data) {
            console.debug("onGetPiontRule done!");
            Actions.getPiontRuleDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            self.parseErrorJson(jqxhr);
            Actions.getPiontRuleFail(jqxhr.appError);
        });
    },

    onModifyPiontRule: function (ProintRule) {
        console.debug("do onModifyPiontRule");
        var self = this;
        var requestData =  ProintRule  ;
        $.ajax({
            type: "POST",
            url: "/Setting/PointRule",
           // contentType: "application/json;charset=utf-8",
            data: requestData,
            dataType: "json"
        }).done(function (data) {
            console.debug("onModifyPiontRule done!", data);
            Actions.modifyPiontRuleDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            self.parseErrorJson(jqxhr);
            Actions.modifyPiontRuleFail(jqxhr.appError);
        });

    },

    onRetrieveMemberPointInfo: function (memberID) {
        console.debug("do onRetrieveMemberPointInfo");
        var self = this;
        $.ajax({
            type: "GET",
            url: "/Mattress/PointExch/" + memberID,
            dataType: "json"
        }).done(function (data) {
            console.debug("onRetrieveMemberPointInfo done!");
            Actions.retrieveMemberPointInfoDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            self.parseErrorJson(jqxhr);
            Actions.retrieveMemberPointInfoFail(jqxhr.appError || jqxhr.responseText);
        });
    },

    onMemberPointExch: function (memberPoint) {
        console.debug("do onMemberPointExch");
        var self = this;
        var requestData = memberPoint;
        $.ajax({
            type: "POST",
            url: "/Mattress/PointExch",
            data: requestData,
            dataType: "json"
        }).done(function (data) {
            console.debug("onMemberPointExch done!", data);
            Actions.memberPointExchDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            self.parseErrorJson(jqxhr);
            Actions.memberPointExchFail(jqxhr.appError || jqxhr.responseText);
        });

    },

    onFindMemberPointDetail: function (memberID, page, pageSize) {
        console.debug("do onFindMemberPointDetail");
        var self = this;
        $.ajax({
            type: "GET",
            url: "/Mattress/PointDetail/" + memberID + "/" + page + "/" + pageSize,
            dataType: "json"
        }).done(function (data) {
            console.debug("onFindMemberPointDetail done!");
            Actions.findMemberPointDetailDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            self.parseErrorJson(jqxhr);
            Actions.findMemberPointDetailFail(jqxhr.appError || jqxhr.responseText);
        });

    },

    onRetrieveMemberRelationInfo: function (memberID) {
        console.debug("do onRetrieveMemberRelationInfo");
        var self = this;
        $.ajax({
            type: "GET",
            url: "/Mattress/MemberRelationDetail/" + memberID,
            dataType: "json"
        }).done(function (data) {
            console.debug("onRetrieveMemberRelationInfo done!");
            Actions.retrieveMemberRelationInfoDone(data);
        }).fail(function (jqxhr, textStatus, errorThrown) {
            self.parseErrorJson(jqxhr);
            Actions.retrieveMemberRelationInfoFail(jqxhr.appError || jqxhr.responseText);
        });

    },

    parseErrorJson: function (jqxhr) {
        console.debug("parseErrorJson Content-Type = ", jqxhr.getResponseHeader('Content-Type'));
        if (/application\/json/.test(jqxhr.getResponseHeader('Content-Type'))) {
            try {
                jqxhr.appError = $.parseJSON(jqxhr.responseText);
            } catch (e) {
                console.debug(e);
            }
        }
    }


});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = accountStore;
}
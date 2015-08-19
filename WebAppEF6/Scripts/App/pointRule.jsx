var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");

var pointRule = React.createClass({

    mixins: [Reflux.ListenerMixin],

    getInitialState: function () {
        return {
            ProintRule: null
        };
    },

    onModifyPiontRuleDone: function (data) {
        alert("修改成功！");
    },

    onModifyPiontRuleFail: function (data) {
        console.debug("onSellMattressFail", data);

        var msg = "";
        if ("Cannot parse request context." === data) {
            msg = "数据传输错误！";
        }

        alert("修改失败！" + msg);
    },

    onGetPiontRuleDone: function (data) {
        console.debug("onGetPiontRuleDone", data);
        if (!data || data.length == 0) {
            alert("没找到积分规则！");
        }

        this.setState({ProintRule:data });
    },

    onGetPiontRuleFail: function (data) {
        console.debug("onGetPiontRuleFail", data);
        alert("获取信息出错！");
    },

    componentWillMount: function () {
       this.listenTo(Actions.getPiontRuleDone, this.onGetPiontRuleDone);
       this.listenTo(Actions.getPiontRuleFail, this.onGetPiontRuleFail);
       this.listenTo(Actions.modifyPiontRuleDone, this.onModifyPiontRuleDone);
       this.listenTo(Actions.modifyPiontRuleFail, this.onModifyPiontRuleFail);
       Actions.getPiontRule();
    },

    componentDidUpdate: function () {
        var L0Self = 0;
        var L0Son = 0;
        var L0Grandson = 0;
        var L1Self = 0;
        var availableAfter = 0;

        if (this.state && this.state.ProintRule) {
            if (this.state.ProintRule.Level0) {
                if (this.state.ProintRule.Level0.SelfRate) {
                    L0Self = this.state.ProintRule.Level0.SelfRate.ValueOfNumber;
                }
                if (this.state.ProintRule.Level0.SonRate) {
                    L0Son = this.state.ProintRule.Level0.SonRate.ValueOfNumber;
                }
                if (this.state.ProintRule.Level0.GrandsonRate) {
                    L0Grandson = this.state.ProintRule.Level0.GrandsonRate.ValueOfNumber;
                }
            }
            if (this.state.ProintRule.Level1) {
                if (this.state.ProintRule.Level1.SelfRate) {
                    L1Self = this.state.ProintRule.Level1.SelfRate.ValueOfNumber;
                }
            }
            if (this.state.ProintRule.AvailableAfter) {

                if (this.state.ProintRule.AvailableAfter.Days) {
                    availableAfter = this.state.ProintRule.AvailableAfter.Days
                } else if (this.state.ProintRule.AvailableAfter.lastIndexOf) {
                    var idx = this.state.ProintRule.AvailableAfter.lastIndexOf(".00:00:00");
                    if (idx > -1) {
                        availableAfter = parseInt(this.state.ProintRule.AvailableAfter.substring(0, idx));
                    } else {
                        availableAfter = parseInt(this.state.ProintRule.AvailableAfter);
                    }
                } else {
                    console.debug("Cannot Parse this.state.ProintRule.AvailableAfter", this.state.ProintRule.AvailableAfter);
                }
                
            }
        }


        React.findDOMNode(this.refs.L0Self).value = L0Self;
        React.findDOMNode(this.refs.L0Son).value = L0Son;
        React.findDOMNode(this.refs.L0Grandson).value = L0Grandson;
        React.findDOMNode(this.refs.L1Self).value = L1Self;
        React.findDOMNode(this.refs.availableAfter).value = availableAfter;
    },

    handleSubmit: function (e) {
        e.preventDefault();
        var L0Self = React.findDOMNode(this.refs.L0Self).value.trim();
        var L0Son = React.findDOMNode(this.refs.L0Son).value.trim();
        var L0Grandson = React.findDOMNode(this.refs.L0Grandson).value.trim();
        var L1Self = React.findDOMNode(this.refs.L1Self).value.trim();
        var availableAfter = React.findDOMNode(this.refs.availableAfter).value.trim();

        try{
            L0Self = parseFloat(L0Self);
            L0Son = parseFloat(L0Son);
            L0Grandson = parseFloat(L0Grandson);
            L1Self = parseFloat(L1Self);
            availableAfter = parseInt(availableAfter);
        }catch(e){
            alert("输入不可识别的字符！");
        }
       
        /*
        if (!this.state.ProintRule) {
            alert("获取信息出错！");
            return;
        }*/

        /*
        var ProintRule = {
            "Level0": {
                "SelfRate":
                    {
                        "Type": "%",
                        "ValueOfNumber": L0Self
                    },
                "SonRate":
                {
                    "Type": "%",
                    "ValueOfNumber": L0Son
                },
                "GrandsonRate":
                {
                    "Type": "%",
                    "ValueOfNumber": L0Grandson
                }
            },
            "Level1": {
                "SelfRate": {
                    "Type": "%",
                    "ValueOfNumber": L1Self
                }
            },
            "AvailableAfter": availableAfter + ".00:00:00"
        };


        if(this.state.ProintRule 
            && this.state.ProintRule.AvailableAfter
            && this.state.ProintRule.AvailableAfter.Days) {

            ProintRule.AvailableAfter = {
                Days: availableAfter
            }
        }*/

        var ProintRule = {
            Level0SelfRate: L0Self,
            Level0SonRate: L0Son,
            Level0GrandsonRate: L0Grandson,

            Level1SelfRate: L1Self,

            AvailableAfter:availableAfter
        };
        Actions.modifyPiontRule(ProintRule);
    },

    render: function () {
        return (
          <div className="row">
            <div className="col-md-12">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                      <hr />  
                      <h4>普通会员</h4>
                        <div className="form-group">
                            <label className="col-md-2 control-label" htmlFor="L0Self">购买返积分：</label>
                            <div className="col-md-8">
                                <input className="form-control uneditable-input" type="text" autoFocus
                                id="L0Self" ref="L0Self"/>
                            </div>
                        </div>
                        <div className="form-group">
                            <label className="col-md-2 control-label" htmlFor="L0Son">二级返积分：</label>
                             <div className="col-md-8">
                                <input className="form-control uneditable-input" type="text"
                                       id="L0Son" ref="L0Son" />
                             </div>
                        </div>
                          <div className="form-group">
                                <label className="col-md-2 control-label" htmlFor="L0Grandson">三级返积分：</label>
                                 <div className="col-md-8">
                                    <input className="form-control uneditable-input" type="text"
                                           id="L0Grandson" ref="L0Grandson" />
                                 </div>
                          </div>

                          <hr />
                          <h4>高级会员</h4>
                           <div className="form-group">
                                <label className="col-md-2 control-label" htmlFor="L1Self">购买返积分：</label>
                                <div className="col-md-8">
                                    <input className="form-control uneditable-input" type="text"
                                           id="L1Self" ref="L1Self" />
                                </div>
                           </div>
                          <hr />
                          <h4>积分兑换</h4>

                          <div className="form-group">
                                    <label className="col-md-2 control-label" htmlFor="availableAfter">可兑换时间：</label>
                                    <div className="col-md-8">
                                        <input className="form-control uneditable-input" type="text"
                                               id="availableAfter" ref="availableAfter" />
                                    </div>
                          </div>

                        <div className="form-group">
                            <div className="col-md-offset-2 col-md-10">
                                <button type="submit" className="btn btn-default">保存</button>
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
    module.exports = pointRule;
}
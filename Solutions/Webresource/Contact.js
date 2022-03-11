var appUrl = "https://orgd38959de.crm5.dynamics.com/";
function onFormLoad() {
    Xrm.Page.getAttribute("parentcustomerid").addOnChange(openRecord);
    Xrm.Page.getAttribute("parentcustomerid").addOnChange(setEmployeeNumber);
    //setEmployeeNumber();
}

function openRecord() {
    var recordId = Xrm.Page.getAttribute("parentcustomerid").getValue();
    if (recordId) {
        recordId = recordId[0].id.split('{')[1].split('}')[0];
        var url = appUrl + "/main.aspx?appid=87df6936-4692-ec11-b400-000d3a133e04&pagetype=entityrecord&etn=account&id=" + recordId;
        window.open(url);
    }
}

function setEmployeeNumber() {
    var recordId = Xrm.Page.getAttribute("parentcustomerid").getValue();

    if (!recordId) {
        return;
    }

    recordId = recordId[0].id.split('{')[1].split('}')[0];
    var req = new XMLHttpRequest();
    req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/contacts?$filter=_parentcustomerid_value eq " + recordId + "&$count=true", true);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200) {
                var results = JSON.parse(this.response);
                var recordCount = results["@odata.count"];

                Xrm.Page.getAttribute("employeeid").setValue(recordCount);

                var entity = {};
                entity.numberofemployees = recordCount;

                Xrm.WebApi.online.updateRecord("account", recordId, entity).then(
                    function success(result) {
                        var updatedEntityId = result.id;
                    },
                    function (error) {
                        Xrm.Utility.alertDialog(error.message);
                    }
                );
                
            } else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send();

}

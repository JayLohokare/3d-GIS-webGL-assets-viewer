import React from 'react';
import { inject, observer } from "mobx-react";


const DataView = inject('store')(observer((props) => {

    let dataList = [];
    const ASC = 'ascending';

    function sortByText(a, b, order = ASC) {
        const diff = a.toLowerCase().localeCompare(b.toLowerCase());

        if (order === ASC) {
            return diff;
        }

        return -1 * diff;
    }

    const printPretty = (object, parent) =>{
        if(typeof object == "string"){
            if(object !== null && object !== ""){
                if(parent==null){
                    dataList.push(object);
                }
                else{
                    dataList.push(parent + ': \n' +  object);
                }
            }
        }
        else{
            for(var property in object){
                let propAccessor
                if(parent !== null){
                    propAccessor = parent + "." + property;
                }
                else{
                    propAccessor = property;
                }
                printPretty(object[property], propAccessor);
            }
        }
    }

    printPretty(props.store.dataPanelContent, null)

    let dataDisplay = dataList.sort((a, b) => sortByText(a, b, ASC)).map((i, k) => {
        return <p key={`${i}-${k}`}>{i}</p>
    });


    return (
        <div className="data-view-card dataOverLay ">
            <div className="data-view-container">
                <div className="row ">
                    <div className="col l12 s12">
                        <h5>Asset Data</h5>
                        {dataDisplay}
                        {
                        props.store.dataPanelContent.AssetId == null ?
                        <p></p>
                        : <button className="center waves-effect waves-light btn">View</button>
                        }
                    </div>
                </div>
            </div>
        </div>
    )
}))

export default DataView;

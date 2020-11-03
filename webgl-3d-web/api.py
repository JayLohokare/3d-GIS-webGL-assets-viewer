from flask import Flask, jsonify, request
from flask_cors import CORS, cross_origin

app = Flask(__name__)
cors = CORS(app, resources={r"/assets": {"origins": "*"}})
app.config['CORS_HEADERS'] = 'Content-Type'


@app.route('/', methods=['GET'])
def health():
    return "Alive"


# {
#     "CreatedBy": "WebGL",
#     "CreatedBySystem": "3D",
#     "EventType": "3D_GET_ASSETS_IN_VIEWPORT",
#     "Message": {
#         "Viewport": [],
#         "Date": "",
#         "Status": ""
#     }
# }

@app.route('/assets', methods=['GET', 'POST'])
def getModels():
    assets = {
        "message": [
            {
                "AssetId": "bee08b85-4e6f-41de-8549-5c99bba4ec94",
                "Master": {
                    "Type":   "Tower",
                    "Description":   "Tower #2",
                    "Equipment": "",
                },
                "Physical": {
                    "EquipmentLength":   "",
                    "EquipmentLengthUom":   "",
                    "LinearReferenceUom":   "",
                    "ReferencePrecision":   "",
                    "GgeographicalReference":   "",
                    "InspectionDirection":   "",
                    "LinearEquipmentType":   "",
                    "Direction":   "",
                    "HardwareVersion":   "",
                    "SoftwareVersion":   "",
                    "OemSiteSystemId":   "",
                    "Vendor":   ""
                },
                "Design": {
                    "ModelURL":  "http://localhost:3000/models/tower.obj"
                },
                "Operation": {},
                "Geospatial": {
                    "XCoordinate":   40.7746,
                    "YCoordinate": -76.245412,
                    "ZCoordinate": -40.0,
                    "XRotation":   0.0,
                    "YRotation": -90.0,
                    "ZRotation":   0.0,
                    "Scale":   0.5
                },
                "Relationships": {},
                "History": {}
            },
            {
                "AssetId": "bee08b85-4e6f-41de-8549-5c99bba4ec94",
                "Master": {
                    "Type":   "Tower",
                    "Description":   "Tower #2",
                    "Equipment": "",
                },
                "Physical": {
                    "EquipmentLength":   "",
                    "EquipmentLengthUom":   "",
                    "LinearReferenceUom":   "",
                    "ReferencePrecision":   "",
                    "GgeographicalReference":   "",
                    "InspectionDirection":   "",
                    "LinearEquipmentType":   "",
                    "Direction":   "",
                    "HardwareVersion":   "",
                    "SoftwareVersion":   "",
                    "OemSiteSystemId":   "",
                    "Vendor":   ""
                },
                "Design": {
                    "ModelURL":  "http://localhost:3000/models/tower.obj"
                },
                "Operation": {},
                "Geospatial": {
                    "XCoordinate":   40.7746,
                    "YCoordinate": -76.245412,
                    "ZCoordinate": -40.0,
                    "XRotation":   0.0,
                    "YRotation": -90.0,
                    "ZRotation":   0.0,
                    "Scale":   0.5
                },
                "Relationships": {},
                "History": {}
            },
            {
                "AssetId": "bee08b85-4e6f-41de-8549-5c99bba4ec94",
                "Master": {
                    "Type":   "Tower",
                    "Description":   "Tower #2",
                    "Equipment": "",
                },
                "Physical": {
                    "EquipmentLength":   "",
                    "EquipmentLengthUom":   "",
                    "LinearReferenceUom":   "",
                    "ReferencePrecision":   "",
                    "GgeographicalReference":   "",
                    "InspectionDirection":   "",
                    "LinearEquipmentType":   "",
                    "Direction":   "",
                    "HardwareVersion":   "",
                    "SoftwareVersion":   "",
                    "OemSiteSystemId":   "",
                    "Vendor":   ""
                },
                "Design": {
                    "ModelURL":  "http://localhost:3000/models/tower.obj"
                },
                "Operation": {},
                "Geospatial": {
                    "XCoordinate":   40.7746,
                    "YCoordinate": -76.245412,
                    "ZCoordinate": -40.0,
                    "XRotation":   0.0,
                    "YRotation": -90.0,
                    "ZRotation":   0.0,
                    "Scale":   0.5
                },
                "Relationships": {},
                "History": {}
            }

        ]
    }

    response = jsonify(assets)
    response.headers.add('Access-Control-Allow-Origin', '*')
    return response


if __name__ == '__main__':
    app.run(host='localhost', debug=True)


############OVERALL###########
# Master
# Physical
# Design
# Geospatial
# Operation
# History
# Relationships

#############Geo Spatial###########
# GisId
# AssetId
# Map
# MapOrg
# Layer
# XCoordinate
# YCoordinate
# ZCoordinate
# //ADD THESE//////////////////////
# XRotation
# YRotation
# ZRotation
# Scale

############ASSET MASTER###########
# Type
# Description

############ASSET PHYSICAL###########
# AssetId
# EquipmentLength
# EquipmentLengthUom
# LinearReferenceUom
# ReferencePrecision
# GgeographicalReference
# InspectionDirection
# LinearEquipmentType
# Direction
# HardwareVersion
# SoftwareVersion
# OemSiteSystemId
# Vendor


############ASSET DESIGN###########
# // ADD THESE//////////////////
# ModelURL

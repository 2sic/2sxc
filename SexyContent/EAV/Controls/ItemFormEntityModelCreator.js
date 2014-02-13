//var EavEntityModels;
//if (EavEntityModels == undefined)
//	EavEntityModels = new Object();
//EavEntityModels[291] = ({"EntityId":291,"EntityGUID":"98de32d0-f01a-4227-ac72-ad0022ca0861","Attributes":[{"StaticName":"LastName","IsTitle":true,"Values":[{"ValueId":1271,"Value":"Gemperle EN","Dimensions":[{"DimensionId":5,"ReadOnly":false}],"ChangeLogIdCreated":168},{"ValueId":1279,"Value":"Gemperle FR","Dimensions":[{"DimensionId":6,"ReadOnly":false}],"ChangeLogIdCreated":170}],"DefaultValue":{"ValueId":1271,"Value":"Gemperle EN","Dimensions":[{"DimensionId":5,"ReadOnly":false}],"ChangeLogIdCreated":168}},{"StaticName":"FirstName","IsTitle":false,"Values":[{"ValueId":1272,"Value":"Benjamin EN","Dimensions":[{"DimensionId":5,"ReadOnly":false}],"ChangeLogIdCreated":168},{"ValueId":1280,"Value":"Benjamin FR","Dimensions":[{"DimensionId":6,"ReadOnly":false}],"ChangeLogIdCreated":170},{"ValueId":1283,"Value":"Benjamin DE","Dimensions":[{"DimensionId":7,"ReadOnly":false}],"ChangeLogIdCreated":171}],"DefaultValue":{"ValueId":1272,"Value":"Benjamin EN","Dimensions":[{"DimensionId":5,"ReadOnly":false}],"ChangeLogIdCreated":168}},{"StaticName":"City","IsTitle":false,"Values":[{"ValueId":1273,"Value":"Sevelen EN","Dimensions":[{"DimensionId":5,"ReadOnly":false}],"ChangeLogIdCreated":168},{"ValueId":1281,"Value":"Sevelen FR","Dimensions":[{"DimensionId":6,"ReadOnly":false}],"ChangeLogIdCreated":170}],"DefaultValue":{"ValueId":1273,"Value":"Sevelen EN","Dimensions":[{"DimensionId":5,"ReadOnly":false}],"ChangeLogIdCreated":168}},{"StaticName":"City2","IsTitle":false,"Values":[{"ValueId":1274,"Value":"Buchs EN","Dimensions":[{"DimensionId":5,"ReadOnly":false}],"ChangeLogIdCreated":168},{"ValueId":1282,"Value":"Buchs FR","Dimensions":[{"DimensionId":6,"ReadOnly":false}],"ChangeLogIdCreated":170}],"DefaultValue":{"ValueId":1274,"Value":"Buchs EN","Dimensions":[{"DimensionId":5,"ReadOnly":false}],"ChangeLogIdCreated":168}}],"FallbackDimensions":[{"DimensionId":5,"ReadOnly":false}]});
//var EavDimensionsModel = [{ "DimensionId": 2, "Name": "English (United States)" }, { "DimensionId": 3, "Name": "Deutsch (Deutschland)" }];

// ToDo: Ensure this is not loaded in Insert-Mode
Eav.Entities = {
    // Creates a client-side model for working with Entities
    CreateEntityModel: function (entityModelJson) {

        var entityModel = {
            EntityId: entityModelJson.EntityId,
            EntityGuid: entityModelJson.EntityGuid,
            Attributes: Eav.Entities.CreateAttributeListModel(entityModelJson),
            _json: entityModelJson
        };

        return entityModel;
    },

    CreateAttributeListModel: function (entityModelJson) {
        var attributes = {};

        $.each(entityModelJson.Attributes, function (i, attributeModelJson) {
            attributes[attributeModelJson.Name] = Eav.Entities.CreateAttributeModel(attributeModelJson);
        });

        return attributes;
    },

    CreateAttributeModel: function (attributeModelJson) {
        var attributeModel = {
            StaticName: attributeModelJson.Name,
            Values: Eav.Entities.CreateValueListModel(attributeModelJson),
            GetDimensions: function () {
                var dimensions = {};
                $.each(attributeModel.Values, function (i, value) {
                    $.extend(dimensions, value.Dimensions);
                });
                return dimensions;
            },
            HasDimension: function (dimensionId) {
                return attributeModel.GetDimensions()[dimensionId] != null;
            },
            GetValueIdByDimensionId: function (dimensionId) {
                var valueId = null;
                $.each(attributeModel.GetDimensions(), function (i, dimension) {
                    if (dimension.DimensionId == dimensionId)
                        valueId = dimension.ValueId;
                });
                return valueId;
            },
            _json: attributeModelJson
        };

        return attributeModel;
    },

    CreateValueListModel: function (attributeModelJson) {
        var values = {};

        $.each(attributeModelJson.Values, function (i, valueModelJson) {
            values[valueModelJson.ValueId] = Eav.Entities.CreateValueModel(valueModelJson);
        });

        return values;
    },

    CreateValueModel: function (valuesModelJson) {
        var valueModel = {
            ValueId: valuesModelJson.ValueId,
            Value: valuesModelJson.TypedContents,
            Dimensions: Eav.Entities.CreateDimensionListModel(valuesModelJson),
            HasDimension: function (dimensionId) {
                return valueModel.Dimensions[dimensionId] != null;
            },
            GetDimensionsCount: function () {
                return $.map(valueModel.Dimensions, function (i, e) {
                    return e;
                }).length;
            },
            _json: valuesModelJson
        };
        return valueModel;
    },

    CreateDimensionListModel: function (valueModelJson) {
        var dimensions = {};

        $.each(valueModelJson.Languages, function (i, dimensionModelJson) {
            dimensions[dimensionModelJson.DimensionId] = Eav.Entities.CreateDimensionModel(dimensionModelJson, valueModelJson.ValueId);
        });

        return dimensions;
    },

    CreateDimensionModel: function (dimensionModelJson, valueId) {
        var dimensionModel = {
            DimensionId: dimensionModelJson.DimensionId,
            ValueId: valueId,
            ReadOnly: dimensionModelJson.ReadOnly,
            _json: dimensionModelJson
        };
        return dimensionModel;
    }
};
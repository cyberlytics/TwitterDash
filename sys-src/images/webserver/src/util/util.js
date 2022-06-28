const gs = require("@google-cloud/scheduler");

export function buildProtoRequest(req, protoReqFields) {
    let protoRequest = {}

    for (let field of protoReqFields) {
        if (req.query.hasOwnProperty(field)) {
            protoRequest[field] = req.query[field];
        }
    }
    return protoRequest;
}

export function convertToProtoTimeStamp(date) {
    return new gs.protos.google.protobuf.Timestamp.fromObject({seconds: Math.floor(date.getTime() / 1000)});
}
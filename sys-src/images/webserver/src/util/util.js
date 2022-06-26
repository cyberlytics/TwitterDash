export function buildProtoRequest(req, protoReqFields) {
    let protoRequest = {}

    for (let field of protoReqFields) {
        if (req.query.hasOwnProperty(field)) {
            protoRequest[field] = req.query[field];
        }
    }
    return protoRequest;
}
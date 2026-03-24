function setBankSlipStatus(payment) {
    let updatedStatus = payment
    let paymentExpired = (payment.ChangedAt > new Date()) ? true : false

    if (payment.Status == 'CREATED') {
        if (paymentExpired) {
            updatedStatus.Status = 'EXPIRED';
            updatedStatus.Reason = "O boleto expirou o tempo de validade";
        } else {
            updatedStatus.Status = 'PENDING';
            updatedStatus.Reason = "Aguardando processamento";
        }
    }
    else if (payment.Status == 'PENDING') {
        if (paymentExpired) {
            updatedStatus.Status = 'EXPIRED';
            updatedStatus.Reason = "O boleto expirou o tempo de validade";
        } else {
            updatedStatus.Status = 'CAPTURED';
            updatedStatus.Reason = "Valor foi efetivamente cobrado";
        }
    }
    else if (payment.Status == 'CAPTURED') {
        updatedStatus.Status = 'SETTLED';
        updatedStatus.Reason = "Valor liquidado para o beneficiário";
    }

    updatedStatus.ChangedAt = new Date()

    return updatedStatus;
}

module.exports = { setBankSlipStatus }
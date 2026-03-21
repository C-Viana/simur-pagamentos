function setPixStatus(payment)
{
    let updatedStatus = payment

    let factor = Math.random()
    let issueRate = 0.2
    let paymentExpired = (payment.ChangedAt > new Date()) ? true : false

    if (payment.Status === 'CREATED')
    {
        if(paymentExpired) {
            updatedStatus.Status = 'EXPIRED';
            updatedStatus.Reason = "O pagamento expirou o tempo de validade";
        }
        else if(factor <= issueRate) {
            updatedStatus.Status = 'REJECTED';
            updatedStatus.Reason = "O pagamento foi rejeitado devido saldo insuficiente";
        }
        else {
            updatedStatus.Status = 'PENDING';
            updatedStatus.Reason = "Aguardando processamento";
        }
    }
    else if (payment.Status === 'PENDING')
    {
        updatedStatus.Status = 'SETTLED';
        updatedStatus.Reason = "Emissor aprovou o valor";
    }

    updatedStatus.ChangedAt = new Date()

    return updatedStatus;
}

module.exports = {setPixStatus}
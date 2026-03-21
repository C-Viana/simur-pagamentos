function setCreditCardStatus(payment)
{
    let updatedStatus = payment

    let factor = Math.random()
    let issueRate = 0.2

    if(factor <= issueRate && payment.Status == 'CREATED')
    {
        updatedStatus.Status = 'FAILED';
        updatedStatus.Reason = "O pagamento não foi autorizado pela operadora";
    }
    else if (factor <= issueRate && payment.Status == 'Autorizado')
    {
        updatedStatus.Status = 'REJECTED';
        updatedStatus.Reason = "O pedido foi cancelado pela operadora";
    }
    else
    {
        if (payment.Status == 'CREATED')
        {
            updatedStatus.Status = 'PENDING';
            updatedStatus.Reason = "Aguardando processamento";
        }
        else if (payment.Status == 'PENDING')
        {
            updatedStatus.Status = 'PROCESSING';
            updatedStatus.Reason = "Emissor aprovou o valor";
        }
        else if (payment.Status == 'PROCESSING')
        {
            updatedStatus.Status = 'AUTHORIZED';
            updatedStatus.Reason = "Emissor aprovou o valor";
        }
        else if (payment.Status == 'AUTHORIZED')
        {
            updatedStatus.Status = 'CAPTURED';
            updatedStatus.Reason = "Valor foi efetivamente cobrado";
        }
        else if (payment.Status == 'CAPTURED')
        {
            updatedStatus.Status = 'SETTLED';
            updatedStatus.Reason = "Valor liquidado para o merchant";
        }
    }

    updatedStatus.ChangedAt = new Date()

    return updatedStatus;
}

module.exports = {setCreditCardStatus}
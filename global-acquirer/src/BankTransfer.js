function setTransferStatus(payment)
{
    let updatedStatus = payment

    let factor = Math.random()
    let issueRate = 0.2

    if(factor <= issueRate && payment.Status == 'CREATED')
    {
        updatedStatus.Status = 'FAILED';
        updatedStatus.Reason = "O pagamento não foi autorizado pela operadora";
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
            updatedStatus.Status = 'SETTLED';
            updatedStatus.Reason = "Valor liquidado para o beneficiário";
        }
    }

    updatedStatus.ChangedAt = new Date()

    return updatedStatus;
}
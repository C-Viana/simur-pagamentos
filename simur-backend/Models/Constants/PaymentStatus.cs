using System.ComponentModel;

namespace simur_backend.Models.Constants
{
    public enum PaymentStatus
    {
        [Description("Criado")] CREATED,
        [Description("Pendente")] PENDING,
        [Description("Processando")] PROCESSING,
        [Description("Autorizado")] AUTHORIZED,
        [Description("Efetivado")] CAPTURED,
        [Description("Finalizado")] SETTLED,
        [Description("Rejeitado")] REJECTED,
        [Description("Cancelado")] CANCELLED,
        [Description("Falha")] FAILED,
        [Description("Bloqueado")] BLOCKED,
        [Description("Expirado")] EXPIRED,
        [Description("Pendente de Reembolso")] REFUND_PENDING,
        [Description("Reembolsado")] REFUNDED,
        [Description("Parcialmente Reembolsado")] PARTIALLY_REFUNDED,
        [Description("Falha de Reembolso")] REFUND_FAILED,
        [Description("Em Disputa")] UNDER_DISPUTE,
        [Description("Contestado")] CHARGEBACK,
        [Description("Contestação Revertida")] CHARGEBACK_REVERSED,
        [Description("Sob Análise")] UNDER_ANALYSIS,
        [Description("Pagamento Excedido")] OVERPAID,
        [Description("Pagamento Incompleto")] UNDERPAID,
        [Description("Pendência do Cliente")] REQUIRES_ACTION
    }
}

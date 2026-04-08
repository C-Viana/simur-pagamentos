import React from 'react';
import {Link} from 'react-router-dom'
import {FiPower} from 'react-icons/fi'
import './styles.css'

export default function Payment() {
    return (
        <div className="payment-container">
            <header>
                <span>Your Payments, username</span>
                <Link className='button' to='book/find'>Refresh list</Link>
                <button type="button">
                    <FiPower size={18} color='#251fc5'/>
                </button>
            </header>
            <h1>Payments</h1>
            <ul>
                <li>
                    <strong>Pedido: </strong>   <p>PF-20260403165626-9853</p>
                    <strong>Valor: </strong>    <p>R$ 999,99</p>
                    <strong>Data: </strong>     <p>24/03/2026</p>
                    <strong>Situação: </strong> <p>Finalizado</p>
                    <button type="button">Detalhes</button>
                </li>
            </ul>
        </div>
    );
}
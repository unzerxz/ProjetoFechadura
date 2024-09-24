import React, { useState, useEffect } from 'react';
import api from '../../services/api';

function ViewHistory() {
    const [history, setHistory] = useState([]);
    const [message, setMessage] = useState('');

    useEffect(() => {
        const fetchHistory = async () => {
            try {
                const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZEZ1bmNpb25hcmlvIjoiMiIsImlzQWRtaW4iOiJ0cnVlIiwiZXhwIjoxNzI3MjcwNjE5LCJpc3MiOiJQcm9qZXRvRmVjaGFkdXJhIiwiYXVkIjoiUHJvamV0b0ZlY2hhZHVyYUF1ZGllbmNlIn0.fQppRIs0NTE3z8b_XxHu5mmEBifxkUR84hlxRwoxLiQ'; // Adicione seu token de autenticação
                const response = await api.get('/Registro', {
                    headers: {
                        Authorization: `Bearer ${token}` // Inclua o token no cabeçalho
                    }
                });
                setHistory(response.data);
            } catch (error) {
                setMessage('Erro ao buscar histórico: ' + error.message);
            }
        };
        fetchHistory();
    }, []);

    return (
        <div className="admin-container">
            <h1>Histórico</h1>
            {message && <p className="message">{message}</p>}
            <ul className="history-list">
                {history.map(record => (
                    <li key={record.idHistorico} className="history-item">
                        {record.descricao}
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default ViewHistory;
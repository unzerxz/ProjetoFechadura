import React, { useState, useEffect } from 'react';
import api from '../../services/api';

function ViewHistory() {
    const [history, setHistory] = useState([]);
    const [message, setMessage] = useState('');

    useEffect(() => {
        const fetchHistory = async () => {
            try {
                const response = await api.get('/Registro');
                setHistory(response.data);
            } catch (error) {
                setMessage('Erro ao buscar histórico: ' + error.message);
            }
        };
        fetchHistory();
    }, []);

    const handleDelete = async (id) => {
        try {
            await api.delete(`/Registro/${id}`);
            setHistory(history.filter(record => record.idRegistro !== id));
            setMessage('Registro deletado com sucesso.');
        } catch (error) {
            setMessage('Erro ao deletar registro: ' + error.message);
        }
    };

    return (
        <div className="admin-container">
            <h1>Histórico</h1>
            {message && <p className="message">{message}</p>}
            <table className="table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Horário de Entrada</th>
                        <th>Horário de Saída</th>
                        <th>ID Funcionário</th>
                        <th>ID Sala</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {history.map(record => (
                        <tr key={record.idRegistro}>
                            <td>{record.idRegistro}</td>
                            <td>{record.horarioEntrada}</td>
                            <td>{record.horarioSaida || 'N/A'}</td>
                            <td>{record.funcionario_IdFuncionario}</td>
                            <td>{record.sala_IdSala}</td>
                            <td>
                                <button onClick={() => handleDelete(record.idRegistro)} className="button delete-button">Deletar</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default ViewHistory;
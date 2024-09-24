import React, { useState, useEffect } from 'react';
import api from '../../services/api';

function ManageEmployees() {
    const [employees, setEmployees] = useState([]);
    const [message, setMessage] = useState('');

    useEffect(() => {
        const fetchEmployees = async () => {
            const response = await api.get('/Funcionario');
            setEmployees(response.data);
        };
        fetchEmployees();
    }, []);

    const handleDelete = async (id) => {
        try {
            await api.delete(`/Funcionario/${id}`);
            setEmployees(employees.filter(e => e.idFuncionario !== id));
            setMessage('Funcionário deletado com sucesso.');
        } catch (err) {
            setMessage('Erro ao deletar funcionário.');
        }
    };

    return (
        <div className="admin-container">
            <h1>Gerenciar Funcionários</h1>
            {message && <p className="message">{message}</p>}
            <ul className="employee-list">
                {employees.map(employee => (
                    <li key={employee.idFuncionario} className="employee-item">
                        {employee.nomeUsuario}
                        <button onClick={() => handleDelete(employee.idFuncionario)} className="delete-button">Deletar</button>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default ManageEmployees;
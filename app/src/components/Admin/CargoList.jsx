import React, { useState, useEffect } from 'react';
import api from '../../services/api';

function CargoList() {
    const [cargos, setCargos] = useState([]);
    const [message, setMessage] = useState('');

    useEffect(() => {
        const fetchCargos = async () => {
            const response = await api.get('/Cargo');
            setCargos(response.data);
        };
        fetchCargos();
    }, []);

    const handleDelete = async (id) => {
        try {
            await api.delete(`/Cargo/${id}`);
            setCargos(cargos.filter(c => c.idCargo !== id));
            setMessage('Cargo deletado com sucesso.');
        } catch (err) {
            setMessage('Erro ao deletar cargo.');
        }
    };

    return (
        <div className="admin-container">
            <h1>Cargos</h1>
            {message && <p className="message">{message}</p>}
            <ul className="cargo-list">
                {cargos.map(cargo => (
                    <li key={cargo.idCargo} className="cargo-item">
                        {cargo.nomeCargo}
                        <button onClick={() => handleDelete(cargo.idCargo)} className="delete-button">Deletar</button>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default CargoList;
import React, { useState, useEffect } from 'react';
import api from '../../services/api';

function SalaView() {
    const [salas, setSalas] = useState([]);
    const [message, setMessage] = useState('');

    useEffect(() => {
        const fetchSalas = async () => {
            const response = await api.get('/Sala');
            setSalas(response.data);
        };
        fetchSalas();
    }, []);

    return (
        <div className="user-container">
            <h1>Salas</h1>
            {message && <p className="message">{message}</p>}
            <ul className="sala-list">
                {salas.map(sala => (
                    <li key={sala.idSala} className="sala-item">
                        {sala.identificacaoSala} - {sala.status ? 'Ocupada' : 'Disponível'}
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default SalaView;
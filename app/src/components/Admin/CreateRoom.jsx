import React, { useState } from 'react';
import api from '../../services/api';

function CreateRoom() {
    const [identificacaoSala, setIdentificacaoSala] = useState('');
    const [message, setMessage] = useState('');

    const handleCreateRoom = async (e) => {
        e.preventDefault();
        try {
            await api.post('/Sala/criarSala', { IdentificacaoSala: identificacaoSala });
            setMessage('Sala criada com sucesso!');
        } catch (err) {
            setMessage('Erro ao criar sala.');
        }
    };

    return (
        <div className="admin-container">
            <h1>Criar Sala</h1>
            {message && <p className="message">{message}</p>}
            <form onSubmit={handleCreateRoom} className="admin-form">
                <div className="form-group">
                    <label htmlFor="identificacaoSala">Identificação da Sala:</label>
                    <input
                        type="text"
                        id="identificacaoSala"
                        value={identificacaoSala}
                        onChange={(e) => setIdentificacaoSala(e.target.value)}
                        required
                    />
                </div>
                <button type="submit" className="admin-button">Criar Sala</button>
            </form>
        </div>
    );
}

export default CreateRoom;
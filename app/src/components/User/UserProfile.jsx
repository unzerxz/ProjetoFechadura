import React, { useState, useEffect } from 'react';
import api from '../../services/api';

function UserProfile() {
    const [user, setUser] = useState({});
    const [senha, setSenha] = useState('');
    const [credencialCartao, setCredencialCartao] = useState('');
    const [message, setMessage] = useState('');

    useEffect(() => {
        const fetchUser = async () => {
            const response = await api.get('/Funcionario/me');
            setUser(response.data);
        };
        fetchUser();
    }, []);

    const handleUpdate = async (e) => {
        e.preventDefault();
        const data = { senha, credencialCartao };
        try {
            await api.put('/Funcionario/me', data);
            setMessage('Dados atualizados com sucesso!');
        } catch (err) {
            setMessage('Erro ao atualizar dados.');
        }
    };

    return (
        <div className="user-container">
            <h1>Perfil do Usuário</h1>
            {message && <p className="message">{message}</p>}
            <form onSubmit={handleUpdate} className="user-form">
                <div className="form-group">
                    <label>Nome de Usuário:</label>
                    <input type="text" value={user.nomeUsuario} readOnly />
                </div>
                <div className="form-group">
                    <label>Email:</label>
                    <input type="email" value={user.email} readOnly />
                </div>
                <div className="form-group">
                    <label>Senha:</label>
                    <input type="password" value={senha} onChange={(e) => setSenha(e.target.value)} />
                </div>
                <div className="form-group">
                    <label>Credencial do Cartão:</label>
                    <input type="text" value={credencialCartao} onChange={(e) => setCredencialCartao(e.target.value)} />
                </div>
                <button type="submit" className="user-button">Atualizar</button>
            </form>
        </div>
    );
}

export default UserProfile;
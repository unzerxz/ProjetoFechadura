import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../../services/api';

function LoginForm() {
    const [identifier, setIdentifier] = useState('');
    const [senha, setSenha] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const response = await api.post('/Funcionario/login', { Identifier: identifier, Senha: senha });
            const Token = response.data.token;
            console.log(Token);

            localStorage.setItem('token', Token);

            const isAdmin = response.data.isAdmin;
            localStorage.setItem('isAdmin', isAdmin);

            if (isAdmin) {
                navigate('/admin-dashboard');
            } else {
                navigate('/user-dashboard');
            }
        } catch (err) {
            setError('Nome de usuário/email ou senha inválidos.');
        }
    };

    return (
        <div className="auth-container">
            <form onSubmit={handleLogin} className="auth-form">
                <h2>Login</h2>
                <div className="form-group">
                    <label htmlFor="identifier">Nome de Usuário ou Email:</label>
                    <input
                        type="text"
                        id="identifier"
                        value={identifier}
                        onChange={(e) => setIdentifier(e.target.value)}
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="senha">Senha:</label>
                    <input
                        type="password"
                        id="senha"
                        value={senha}
                        onChange={(e) => setSenha(e.target.value)}
                        required
                    />
                </div>
                {error && <p className="error-message">{error}</p>}
                <button type="submit" className="auth-button">Login</button>
            </form>
        </div>
    );
}

export default LoginForm;
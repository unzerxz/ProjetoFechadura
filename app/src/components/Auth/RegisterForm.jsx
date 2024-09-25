import React, { useState } from 'react';
import api from '../../services/api';

function RegisterForm() {
    const [nomeUsuario, setNomeUsuario] = useState('');
    const [email, setEmail] = useState('');
    const [senha, setSenha] = useState('');
    const [nome, setNome] = useState('');
    const [message, setMessage] = useState('');

    const handleRegister = async (e) => {
        e.preventDefault();
        try {
            await api.post('/Funcionario', {Nome: nome, NomeUsuario: nomeUsuario, Email: email, Senha: senha });
            setMessage('Cadastro realizado com sucesso!');
        } catch (err) {
            setMessage('Erro ao realizar cadastro.');
        }
    };

    return (
        <div className="auth-container">
            <form onSubmit={handleRegister} className="auth-form">
                <h2>Cadastro</h2>
                <div className="form-group">
                    <label htmlFor="nomeCompleto">Nome Completo:</label>
                    <input
                        type="text"
                        id="nomeCompleto"
                        value={nome}
                        onChange={(e) => setNome(e.target.value)}
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="nomeUsuario">Nome de Usuário:</label>
                    <input
                        type="text"
                        id="nomeUsuario"
                        value={nomeUsuario}
                        onChange={(e) => setNomeUsuario(e.target.value)}
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="email">Email:</label>
                    <input
                        type="email"
                        id="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
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
                {message && <p className="message">{message}</p>}
                <button type="submit" className="auth-button">Cadastrar</button>
            </form>
        </div>
    );
}

export default RegisterForm;

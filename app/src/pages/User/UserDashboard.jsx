import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import api from '../../services/api';

function UserDashboard() {
    const navigate = useNavigate();
    const employeeId = localStorage.getItem('employeeId'); // Get employee ID from localStorage

    // State for user data and modal visibility
    const [nomeCompleto, setNomeCompleto] = useState('');
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [credencialCartao, setCredencialCartao] = useState('');
    const [credencialTeclado, setCredencialTeclado] = useState('');
    const [isAtivo, setIsAtivo] = useState('');
    const [cargoId, setCargoId] = useState('');
    const [perfilId, setPerfilId] = useState('');
    const [idFuncionario, setIdFuncionario] = useState('');
    const [showModal, setShowModal] = useState(false);

    // Fetch user's information using their ID
    const fetchUserDetails = async () => {
        try {
            const response = await api.get(`/Funcionario/${employeeId}`);
            const userData = response.data;
            setIdFuncionario(userData.idFuncionario);
            setNomeCompleto(userData.nome);
            setUsername(userData.nomeUsuario);
            setEmail(userData.email);
            setPassword(userData.senha);
            setCredencialCartao(userData.credencialCartao);
            setCredencialTeclado(userData.credencialTeclado);
            setIsAtivo(userData.isAtivo);
            setCargoId(userData.cargo_IdCargo);
            setPerfilId(userData.perfil_IdPerfil);
        } catch (err) {
            console.error('Erro ao buscar informações do usuário:', err);
        }
    };

    // Call fetchUserDetails when the component mounts
    useEffect(() => {
        fetchUserDetails();
    }, []);

    const handleLogout = async () => {
        try {
            await api.post('/Funcionario/logout');
            localStorage.removeItem('token');
            localStorage.removeItem('employeeId');
            localStorage.removeItem('isAdmin');
            navigate('/login');
        } catch (err) {
            console.error('Erro ao deslogar:', err);
        }
    };

    const handleEdit = () => {
        setShowModal(true); // Open the modal
    };

    const handleSaveChanges = async () => {
        try {
            // Sending the entire user object as per the API requirements
            await api.put(`/Funcionario/${employeeId}`, {
                idFuncionario: idFuncionario,
                nome: nomeCompleto,
                nomeUsuario: username,
                email: email,
                senha: password,
                credencialCartao: credencialCartao,
                credencialTeclado: credencialTeclado,
                isAtivo: isAtivo,
                cargo_IdCargo: cargoId,
                perfil_IdPerfil: perfilId
            });
            setShowModal(false); // Close modal after successful update
        } catch (err) {
            console.error('Erro ao atualizar as informações:', err);
        }
    };

    return (
        <div className="user-dashboard">
            <h1>User Dashboard</h1>
            <p>Bem-vindo, {nomeCompleto}!</p>
            <ul className="user-menu">
                <li><Link to="/salas">Ver Salas</Link></li>
            </ul>
            <button onClick={handleEdit} className="edit-info-button">Editar Informações</button>
            <button onClick={handleLogout} className="logout-button">Logout</button>

            {showModal && (
                <div className="modal">
                    <div className="modal-content">
                        <h2>Editar Informações</h2>
                        <label>Nome Completo:</label>
                        <input
                            type="text"
                            value={nomeCompleto}
                            onChange={(e) => setNomeCompleto(e.target.value)}
                        />
                        <label>Username:</label>
                        <input
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                        />
                        <label>Email:</label>
                        <input
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                        <label>Senha:</label>
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                        />
                        <button onClick={handleSaveChanges}>Salvar</button>
                        <button onClick={() => setShowModal(false)}>Fechar</button>
                    </div>
                </div>
            )}
        </div>
    );
}

export default UserDashboard;

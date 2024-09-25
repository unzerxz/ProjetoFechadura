import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import api from '../../services/api';

function AdminDashboard() {
    const navigate = useNavigate();
    const employeeId = localStorage.getItem('employeeId'); // Get employee ID from localStorage

    // State for user data and modal visibility
    const [nomeCompleto, setNomeCompleto] = useState('');
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [idFuncionario, setIdFuncionario] = useState('');
    const [credencialCartao, setCredencialCartao] = useState('');
    const [credencialTeclado, setCredencialTeclado] = useState('');
    const [isAtivo, setIsAtivo] = useState('');
    const [cargoId, setCargoId] = useState('');
    const [perfilId, setPerfilId] = useState('');
    const [password, setPassword] = useState('');
    const [showModal, setShowModal] = useState(false);

    // Fetch employee's information using their ID
    const fetchEmployeeDetails = async () => {
        try {
            const response = await api.get(`/Funcionario/${employeeId}`);
            const employeeData = response.data;
            setIdFuncionario(employeeData.idFuncionario)
            setCredencialTeclado(employeeData.credencialTeclado)
            setCredencialCartao(employeeData.credencialCartao)
            setIsAtivo(employeeData.isAtivo)
            setPassword(employeeData.senha)
            setCargoId(employeeData.cargo_IdCargo)
            setPerfilId(employeeData.perfil_IdPerfil)
            setNomeCompleto(employeeData.nome);
            setUsername(employeeData.nomeUsuario);
            setEmail(employeeData.email); // Assuming the API provides email
        } catch (err) {
            console.error('Erro ao buscar informações do funcionário:', err);
        }
    };

    // Call fetchEmployeeDetails when the component mounts
    useEffect(() => {
        fetchEmployeeDetails();
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
            await api.put(`/Funcionario/${employeeId}`, {
                "idFuncionario": idFuncionario,
                "nome": nomeCompleto,
                "nomeUsuario": username,
                "email": email,
                "credencialCartao": credencialCartao,
                "credencialTeclado": credencialTeclado,
                "senha": password,
                "isAtivo": isAtivo,
                "cargo_IdCargo": cargoId,
                "perfil_IdPerfil": perfilId
            });
            setShowModal(false); // Close modal after successful update
        } catch (err) {
            console.error('Erro ao atualizar as informações:', err);
        }
    };

    return (
        <div className="admin-dashboard">
            <h1>Admin Dashboard</h1>
            <p>Bem-vindo, {nomeCompleto}!</p>
            <ul className="admin-menu">
                <li><Link to="/admin/cargos">Gerenciar Cargos</Link></li>
                <li><Link to="/admin/salas">Criar Sala</Link></li>
                <li><Link to="/admin/funcionarios">Gerenciar Funcionários</Link></li>
                <li><Link to="/admin/historico">Ver Histórico</Link></li>
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

export default AdminDashboard;

import React, { useState, useEffect } from 'react';
import api from '../../services/api';

function ManageEmployees() {
    const [employees, setEmployees] = useState([]);
    const [message, setMessage] = useState('');
    const [selectedEmployee, setSelectedEmployee] = useState(null);
    const [cargos, setCargos] = useState([]);
    const [perfis, setPerfis] = useState([]);
    const [showModal, setShowModal] = useState(false);
    const [isUpdateModal, setIsUpdateModal] = useState(false); // Para diferenciar entre validar e atualizar

    useEffect(() => {
        fetchEmployees(); // Carrega os funcionários ao montar o componente
    }, []);

    useEffect(() => {
        fetchCargos(); // Carrega os cargos
        fetchPerfis(); // Carrega os perfis
    }, []);

    const fetchEmployees = async () => {
        const response = await api.get('/Funcionario');
        setEmployees(response.data);
    };

    const fetchCargos = async () => {
        const response = await api.get('/Cargo'); // Endpoint para buscar cargos
        setCargos(response.data);
    };

    const fetchPerfis = async () => {
        const response = await api.get('/Perfil'); // Endpoint para buscar perfis
        setPerfis(response.data);
    };

    const handleDelete = async (id) => {
        try {
            await api.delete(`/Funcionario/${id}`);
            setMessage('Funcionário deletado com sucesso.');
            fetchEmployees(); // Atualiza a lista de funcionários
        } catch (err) {
            setMessage('Erro ao deletar funcionário.');
        }
    };

    const handleUpdate = (employee) => {
        setSelectedEmployee(employee);
        setIsUpdateModal(true); // Abre o modal de atualização
        setShowModal(true);
    };

    const handleValidate = (employee) => {
        setSelectedEmployee(employee);
        setIsUpdateModal(false); // Abre o modal de validação
        setShowModal(true);
    };

    const handleModalClose = () => {
        setShowModal(false);
        setSelectedEmployee(null);
    };

    const handleValidationSubmit = async () => {
        if (selectedEmployee) {
            try {
                await api.post(`/Funcionario/confirmacaoFuncionario?idFuncionario=${selectedEmployee.idFuncionario}&novoCargoId=${parseInt(selectedEmployee.cargo_IdCargo)}&novoPerfilId=${parseInt(selectedEmployee.perfil_IdPerfil)}`);
                setMessage('Funcionário validado com sucesso.');
                handleModalClose();
                fetchEmployees(); // Atualiza a lista de funcionários
            } catch (error) {
                setMessage('Erro ao validar funcionário: ' + error.message);
            }
        }
    };

    const handleUpdateSubmit = async () => {
        if (selectedEmployee) {
            try {
                await api.put(`/Funcionario/${selectedEmployee.idFuncionario}`, {
                    idFuncionario: selectedEmployee.idFuncionario,
                    nome: selectedEmployee.nome,
                    nomeUsuario: selectedEmployee.nomeUsuario,
                    email: selectedEmployee.email,
                    credencialCartao: selectedEmployee.credencialCartao,
                    credencialTeclado: selectedEmployee.credencialTeclado,
                    senha: selectedEmployee.senha,
                    isAtivo: selectedEmployee.isAtivo ? 1 : 0, // Converte para 1 ou 0
                    cargo_IdCargo: parseInt(selectedEmployee.cargo_IdCargo),
                    perfil_IdPerfil: parseInt(selectedEmployee.perfil_IdPerfil),
                });
                setMessage('Funcionário atualizado com sucesso.');
                handleModalClose();
                fetchEmployees(); // Atualiza a lista de funcionários
            } catch (error) {
                setMessage('Erro ao atualizar funcionário: ' + error.message);
            }
        }
    };

    return (
        <div className="admin-container">
            <h1>Gerenciar Funcionários</h1>
            {message && <p className="message">{message}</p>}
            <table className="table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nome</th>
                        <th>Nome de Usuário</th>
                        <th>Email</th>
                        <th>Senha</th>
                        <th>Ativo</th>
                        <th>Credencial Cartão</th>
                        <th>Credencial Teclado</th>
                        <th>Cargo ID</th>
                        <th>Perfil ID</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {employees.map(employee => (
                        <tr key={employee.idFuncionario}>
                            <td>{employee.idFuncionario}</td>
                            <td>{employee.nome}</td>
                            <td>{employee.nomeUsuario}</td>
                            <td>{employee.email}</td>
                            <td>{employee.senha}</td>
                            <td>{employee.isAtivo ? 'Sim' : 'Não'}</td>
                            <td>{employee.credencialCartao}</td>
                            <td>{employee.credencialTeclado}</td>
                            <td>{employee.cargo_IdCargo}</td>
                            <td>{employee.perfil_IdPerfil}</td>
                            <td>
                                <button onClick={() => handleDelete(employee.idFuncionario)} className="button delete-button">Deletar</button>
                                <button onClick={() => handleUpdate(employee)} className="button update-button">Atualizar</button>
                                <button onClick={() => handleValidate(employee)} className="button validate-button">Validar</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {showModal && (
                <div className="modal">
                    <div className="modal-content">
                        <span className="close" onClick={handleModalClose}>&times;</span>
                        <h2>{isUpdateModal ? 'Atualizar Funcionário' : 'Validar Funcionário'}</h2>
                        {selectedEmployee && (
                            <div>
                                <p>ID: {selectedEmployee.idFuncionario}</p>
                                <label>
                                    Nome:
                                    <input
                                        type="text"
                                        value={selectedEmployee.nome}
                                        onChange={(e) => setSelectedEmployee({ ...selectedEmployee, nome: e.target.value })}
                                        disabled={isUpdateModal ? false : true} // Desabilita se não for atualização
                                    />
                                </label>
                                <label>
                                    Nome de Usuário:
                                    <input
                                        type="text"
                                        value={selectedEmployee.nomeUsuario}
                                        onChange={(e) => setSelectedEmployee({ ...selectedEmployee, nomeUsuario: e.target.value })}
                                        disabled={isUpdateModal ? false : true} // Desabilita se não for atualização
                                    />
                                </label>
                                <label>
                                    Email:
                                    <input
                                        type="email"
                                        value={selectedEmployee.email}
                                        onChange={(e) => setSelectedEmployee({ ...selectedEmployee, email: e.target.value })}
                                        disabled={isUpdateModal ? false : true} // Desabilita se não for atualização
                                    />
                                </label>
                                <label>
                                    Senha:
                                    <input
                                        type="password"
                                        value={selectedEmployee.senha}
                                        onChange={(e) => setSelectedEmployee({ ...selectedEmployee, senha: e.target.value })}
                                        disabled={isUpdateModal ? false : true} // Desabilita se não for atualização
                                    />
                                </label>
                                <label>
                                    Ativo:
                                    <input
                                        type="checkbox"
                                        checked={selectedEmployee.isAtivo === 1}
                                        onChange={(e) => setSelectedEmployee({ ...selectedEmployee, isAtivo: e.target.checked ? 1 : 0 })}
                                        disabled={isUpdateModal ? false : true} // Desabilita se não for atualização
                                    />
                                </label>
                                <label>
                                    Cargo:
                                    <select
                                        value={selectedEmployee.cargo_IdCargo}
                                        onChange={(e) => setSelectedEmployee({ ...selectedEmployee, cargo_IdCargo: e.target.value })}
                                    >
                                        {cargos.map(cargo => (
                                            <option key={cargo.idCargo} value={cargo.idCargo}>{cargo.nomeCargo}</option>
                                        ))}
                                    </select>
                                </label>
                                <label>
                                    Perfil:
                                    <select
                                        value={selectedEmployee.perfil_IdPerfil}
                                        onChange={(e) => setSelectedEmployee({ ...selectedEmployee, perfil_IdPerfil: e.target.value })}
                                    >
                                        {perfis.map(perfil => (
                                            <option key={perfil.idPerfil} value={perfil.idPerfil}>{perfil.tipoPerfil}</option>
                                        ))}
                                    </select>
                                </label>
                                {isUpdateModal ? (
                                    <button onClick={handleUpdateSubmit} className="button validate-button">Confirmar Atualização</button>
                                ) : (
                                    <button onClick={handleValidationSubmit} className="button validate-button">Confirmar Validação</button>
                                )}
                            </div>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
}

export default ManageEmployees;
import React, { useState, useEffect } from 'react';
import api from '../../services/api';

function CargoList() {
    const [cargos, setCargos] = useState([]);
    const [message, setMessage] = useState('');
    const [selectedCargo, setSelectedCargo] = useState(null);
    const [showModal, setShowModal] = useState(false);
    const [isUpdateModal, setIsUpdateModal] = useState(false);

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

    const handleAdd = () => {
        setSelectedCargo({ nomeCargo: '' });
        setIsUpdateModal(false);
        setShowModal(true);
    };

    const handleUpdate = (cargo) => {
        setSelectedCargo(cargo);
        setIsUpdateModal(true);
        setShowModal(true);
    };

    const handleModalClose = () => {
        setShowModal(false);
        setSelectedCargo(null);
    };

    const handleSave = async () => {
        try {
            if (isUpdateModal) {
                await api.put(`/Cargo/${selectedCargo.idCargo}`, selectedCargo);
                setMessage('Cargo atualizado com sucesso.');
            } else {
                await api.post(`/Cargo`, selectedCargo);
                setMessage('Cargo adicionado com sucesso.');
            }
            setShowModal(false);
            setSelectedCargo(null);
            const response = await api.get('/Cargo');
            setCargos(response.data); // Refresh cargo list
        } catch (err) {
            setMessage('Erro ao salvar o cargo.');
        }
    };

    return (
        <div className="admin-container">
            <h1>Cargos</h1>
            {message && <p className="message">{message}</p>}

            <button onClick={handleAdd} className="add-button">Adicionar Novo Cargo</button>
            
            <ul className="cargo-list">
                {cargos.map(cargo => (
                    <li key={cargo.idCargo} className="cargo-item">
                        {cargo.nomeCargo}
                        <button onClick={() => handleUpdate(cargo)} className="update-button">Atualizar</button>
                        <button onClick={() => handleDelete(cargo.idCargo)} className="delete-button">Deletar</button>
                    </li>
                ))}
            </ul>

            {showModal && (
                <div className="modal">
                    <div className="modal-content">
                        <span className="close" onClick={handleModalClose}>&times;</span>
                        <h2>{isUpdateModal ? 'Atualizar Cargo' : 'Adicionar Cargo'}</h2>
                        <label>
                            Nome do Cargo:
                            <input
                                type="text"
                                value={selectedCargo.nomeCargo}
                                onChange={(e) => setSelectedCargo({ ...selectedCargo, nomeCargo: e.target.value })}
                            />
                        </label>
                        <button onClick={handleSave} className="save-button">
                            {isUpdateModal ? 'Confirmar Atualização' : 'Adicionar Cargo'}
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
}

export default CargoList;

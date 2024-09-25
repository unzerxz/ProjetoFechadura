import React, { useState, useEffect } from 'react';
import api from '../../services/api';

function SalaView() {
    const [salas, setSalas] = useState([]);
    const [message, setMessage] = useState('');

    useEffect(() => {
        const fetchSalas = async () => {
            try {
                const response = await api.get('/Sala');
                const salasData = response.data;

                // Para cada sala, se estiver ocupada, buscar o nome do funcionário
                const updatedSalas = await Promise.all(salasData.map(async (sala) => {
                    if (sala.status === 1 && sala.funcionario_IdFuncionario) { // Se a sala estiver ocupada e idFuncionario for válido
                        try {
                            const funcionarioResponse = await api.get(`/Funcionario/${sala.funcionario_IdFuncionario}`);
                            console.log(funcionarioResponse.data);
                            console.log(funcionarioResponse.data.nome);
                            return {
                                ...sala,
                                funcionarioNome: funcionarioResponse.data.nome // Adiciona o nome do funcionário
                            };
                        } catch (error) {
                            console.error(`Erro ao buscar funcionário com ID ${sala.funcionario_IdFuncionario}:`, error);
                            return {
                                ...sala,
                                funcionarioNome: 'Desconhecido' // Se houver erro, define como desconhecido
                            };
                        }
                    }
                    return {
                        ...sala,
                        funcionarioNome: null // Se não estiver ocupada, não há nome
                    };
                }));

                setSalas(updatedSalas);
            } catch (error) {
                setMessage('Erro ao carregar salas: ' + error.message);
            }
        };

        fetchSalas(); // Chama a função uma vez ao montar o componente

        const interval = setInterval(fetchSalas, 5000); // Verifica a cada 5 segundos

        return () => clearInterval(interval); // Limpa o intervalo ao desmontar o componente
    }, []);

    return (
        <div className="user-container">
            <h1>Salas</h1>
            {message && <p className="message">{message}</p>}
            <div className="room-grid">
                {salas.map(sala => (
                    <div key={sala.idSala} className={`sala-card ${sala.status === 1 ? 'occupied' : 'available'}`}>
                        <h3>{sala.identificacaoSala}</h3>
                        <p>Status: {sala.status === 1 ? 'Ocupada' : 'Disponível'}</p>
                        {sala.status === 1 && <p>Ocupada por: {sala.funcionarioNome || 'Desconhecido'}</p>} {/* Exibe o nome do funcionário */}
                    </div>
                ))}
            </div>
        </div>
    );
}

export default SalaView;
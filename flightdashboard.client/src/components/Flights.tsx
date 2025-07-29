import React, { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import  '../Style/StyleSheet.css';
import { Flight } from '../models/Flight';



const Flights: React.FC = () => {
    const [flights, setFlights] = useState<Flight[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [showAddingFlight, setShowAddingFlight] = useState<boolean>(true);
    const URL = "https://localhost:7185/hubs/flight";
    const apiBase = "https://localhost:7185/api/Flights";


    // Add a new flight
    const addFlight = async (flight: Omit<Flight, 'id'>) => {
        try {
            const response = await fetch(apiBase, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ ...flight, departureTime: new Date(flight.departureTime).toISOString() })
            });

            if (!response.ok) throw new Error('Failed to add flight');
            const newFlight = await response.json();
            setFlights(prev => [...prev, newFlight]);
        } catch (error) {
            console.error('Add Flight Error:', error);
        }
        setNewFlightData({
            status: 'Scheduled',
            arrivalAirport: '',
            departureAirport: '',
            arrivalTime: addHours(new Date(), 3),
            departureTime: new Date(),
            flightNumber: ''
        });
    };


    function addHours(date, hours) {
        const hoursToAdd = hours * 60 * 60 * 1000;
        date.setTime(date.getTime() + hoursToAdd);
        return date;
    }

    // NEW state for modal and form
    const [showModal, setShowModal] = useState(false);
    const [newFlightData, setNewFlightData] = useState<Omit<Flight, 'id'>>({
        status: 'Scheduled',
        arrivalAirport: '',
        departureAirport: '',
        departureTime: new Date(),
        arrivalTime: addHours(new Date(), 3),
        flightNumber: ''
    });

    // Handlers
    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setNewFlightData(prev => ({ ...prev, [name]: value }));
    };

    const handleAddFlight = async (e: React.FormEvent) => {
        e.preventDefault();
        await addFlight(newFlightData);
        toggleModal();
    };


    // Update an existing flight
    const updateFlight = async (flight: Flight) => {
        try {
            const response = await fetch(`${apiBase} /${flight.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(flight)
            });

            if (!response.ok) throw new Error('Failed to update flight');
            const updated = await response.json();
            setFlights(prev =>
                prev.map(f => (f.id === updated.id ? updated : f))
            );
        } catch (error) {
            console.error('Update Flight Error:', error);
        }
    };

    // Delete a flight by ID
    const deleteFlight = async (id: number) => {
        try {
            const response = await fetch(`${apiBase}/${id}`, {
                method: 'DELETE'
            });

            if (!response.ok) throw new Error('Failed to delete flight');
            setFlights(prev => prev.filter(f => f.id !== id));
        } catch (error) {
            console.error('Delete Flight Error:', error);
        }
    };

    useEffect(() => {
        let connection: signalR.HubConnection;

        async function fetchFlights() {
            setLoading(true);
            try {
                const response = await fetch(apiBase);
                const data: Flight[] = await response.json();
                setFlights(data);
            } catch (error) {
                console.error('Error fetching flights:', error);
                setFlights([]);
            } finally {
                setLoading(false);
            }
        }

        // SignalR connection setup
        connection = new signalR.HubConnectionBuilder()
            .withUrl(URL)
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => {
                console.log('Connected to SignalR hub');
            })
            .catch(err => console.error('SignalR Connection Error: ', err));

        connection.on('FlightAdded', (flight: Flight) => {
            setFlights(prev => [...prev, flight]);
        });

        connection.on('FlightUpdated', (updatedFlight: Flight) => {
            setFlights(prev =>
                prev.map(f => (f.id === updatedFlight.id ? updatedFlight : f))
            );
        });

        connection.on('FlightDeleted', (id: number | string) => {
            setFlights(prev => prev.filter(f => f.id !== Number(id)));
        });

        fetchFlights();

        return () => {
            connection.stop();
        };
    }, []);

    const toggleModal = () => {
        setShowModal(!showModal);
        setShowAddingFlight(!showAddingFlight);
    }

    if (loading) {
        return <div>Loading flights...</div>;
    }

    return (
        <div>
            <h2>Flights</h2>

            <table>
                <thead>
                    <tr>
                        <th>Flight Number</th>
                        <th>Departure</th>
                        <th>Arrival</th>
                        <th>Departure Time</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {flights.map(flight => (
                        <tr key={flight?.id}
                            className={`status-${flight?.status?.toLowerCase()}`}
                        >
                            <td>{flight?.flightNumber}</td>
                            <td>{flight?.departureAirport}</td>
                            <td>{flight?.arrivalAirport}</td>
                            <td>{flight?.departureTime}</td>
                            <td>{flight?.status}</td>
                            <td>
                                <button onClick={() => deleteFlight(flight?.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <br />

            <button onClick={() =>
                deleteFlight(1)
            }>Delete Flight with ID 1</button>
            {showModal && (
                <div className="modal-overlay">
                    <div className="modal">
                        <h3>Add New Flight</h3>
                        <form onSubmit={handleAddFlight}>
                            number: <input
                                name="flightNumber"
                                placeholder="Flight Number"
                                value={newFlightData.flightNumber}
                                onChange={handleInputChange}
                                required
                            />
                            departure airport: <input
                                name="departureAirport"
                                placeholder="Departure Airport"
                                value={newFlightData.departureAirport}
                                onChange={handleInputChange}
                                required
                            />
                            arrival airport: <input
                                name="arrivalAirport"
                                placeholder="Arrival Airport"
                                value={newFlightData.arrivalAirport}
                                onChange={handleInputChange}
                                required
                            />
                            departure time:
                            <input
                                type="datetime-local"
                                name="departureTime"
                                value={new Date(newFlightData.departureTime).toISOString().slice(0, 16)}
                                onChange={e =>
                                    setNewFlightData(prev => ({
                                        ...prev,
                                        departureTime: new Date(e.target.value)
                                    }))
                                }
                                required
                            />
                            arrival time:
                            <input
                                type="datetime-local"
                                name="arrivalTime"
                                value={addHours(new Date(), 3).toISOString().slice(0, 16)}
                                onChange={e =>
                                    setNewFlightData(prev => ({
                                        ...prev,
                                        arrivalTime: new Date(e.target.value)
                                    }))
                                }
                                required
                            />
                            status:
                            <select
                                name="status"
                                value={newFlightData.status}
                                onChange={handleInputChange}
                            >
                                <option value="Scheduled">Scheduled</option>
                                <option value="Boarding">Boarding</option>
                                <option value="Departed">Departed</option>
                                <option value="Landed">Landed</option>
                            </select>
                            <br />
                            <button type="submit">Add Flight</button>
                            <button type="button" onClick={() => toggleModal()}>Cancel</button>
                        </form>

                    </div>
                </div>
            )}
            {showAddingFlight ?
                <button onClick={() => toggleModal()}>add flight</button> : <></>
            }




        </div>
    );
};

export default Flights;
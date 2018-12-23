import axios from 'axios';
import moment from 'moment';
import DatePicker from '../DatePicker/DatePicker.vue.html';

export default {
    components: {
        DatePicker
    },
    data: () => ({
        dialog: false,
        headers: [
            { text: 'Порядковый №', align: 'left', value: 'position' },
            { text: 'Название', align: 'left', value: 'name' },
            { text: 'Описание', align: 'left', sortable: false, value: 'description' },
            { text: 'Дата создания', align: 'left', value: 'createdAt' },
            { text: 'Дата редактирования', align: 'left', value: 'modifiedAt' }
        ],
        search: '',
        totalNotes: 0,
        loading: true,
        pagination: {},
        notes: [],
        targets: [],
        cities: [],
        contributions: [],
        editedIndex: -1,
        isAdmin: false,
        editedItem: {
            interestRate: 0,
            additionalContributions: []
        },
        defaultItem: {},
        valid: true
    }),
    mounted() {
    },
    computed: {
        formTitle() {
            return this.editedIndex === -1 ? 'Новая заметка' : 'Редактирование';
        }
    },

    watch: {
        pagination: {
            handler() {
                this.initialize();
            },
            deep: true
        },
        dialog(val) {
            val || this.close();
        }
    },
    filters: {
        russianDate: function (date) {
            if (date)
                return moment(date).format('DD.MM.YYYY HH:mm');
            else
                return '';
        }
    },
    created () {
    },

    methods: {   
        initialize() {
            debugger;
            var cookie = document.cookie;
            if (cookie) {
                var split_read_cookie = cookie.split(";");
                for (var i = 0; i < split_read_cookie.length; i++) {
                    var value = split_read_cookie[i];
                    value = value.split("=");
                    if (value[0] === "IsAdmin" && value[1] === "true") {
                        this.isAdmin = true;
                    }
                }
            }
            this.loading = true;
            var componentData = this;
            const { sortBy, descending, page, rowsPerPage } = this.pagination;
            axios.all([
                axios.post('/api/Note/Get', { Pagination: this.pagination}),
                    axios.post('/api/Note/GetTotalElements', {})
                ])
                .then(axios.spread(function (notes, total) {
                    if (typeof (notes.data) !== "object") {
                        return;
                    }
                    componentData.notes = notes.data;
                    componentData.totalNotes = total.data;
                }))
                .catch(function(error) {
                        console.log(error);
                    }
                );
            this.loading = false;
        },

       editItem (item) {
            this.editedIndex = this.notes.indexOf(item);
            this.editedItem = Object.assign({}, item); 
            if (this.editedItem.createdAt) {
                this.editedItem.createdAtTime = moment(this.editedItem.createdAt).format('HH:mm');
            }
            if (this.editedItem.modifiedAt) {
                this.editedItem.modifiedAtTime = moment(this.editedItem.modifiedAt).format('HH:mm');
            }
            this.dialog = true;
        },

        deleteItem(item) {
            var componentData = this;
            const index = this.notes.indexOf(item);
            confirm('Вы действительно хотите удалить элемент?') &&
                axios.post('/api/Note/Remove?id=' + item.id)
                .then(function (response) {
                    if (typeof (response.data) !== "object") {
                        componentData.notes.splice(index, 1);
                    }
                })
                .catch(function (error) {
                    console.log(error);
                });
        },
        close() {
            this.dialog = false;
            setTimeout(() => {
                    this.editedItem = Object.assign({}, this.defaultItem);
                    this.editedIndex = -1;
                },
                300);
        },
      
        save() {
            this.$validator.validateAll().then((result) => {
                var componentData = this;
                if (result) {
                    if (this.editedIndex > -1) {
                        var note = {};
                        note.id = this.editedItem.id;
                        note.name = this.editedItem.name;
                        note.position = this.editedItem.position;
                        note.description = this.editedItem.description;
                        axios.post('/api/Note/Update', { Note: note})
                            .then(function (response) {
                                if (response.status === 200) {
                                    Object.assign(componentData.notes[componentData.editedIndex], response.data);
                                }
                                componentData.close();
                            })
                            .catch(function (error) {
                                console.log(error);
                                componentData.close();
                            });
                    } else {
                        axios.post('/api/Note/Create', { Note: this.editedItem })
                            .then(function (response) {
                                if (response.status === 200) {
                                    componentData.editedItem = response.data;
                                    componentData.notes.push(componentData.editedItem);
                                }
                                componentData.close();
                            })
                            .catch(function (error) {
                                console.log(error);
                                componentData.close();
                            });
                    }
                    return;
                }
                alert('Ошибки валидации!');
            });

        }
    }
}
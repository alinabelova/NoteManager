$(document).ready(function () {
    $('.modal').modal();
});
var defaultTitle = "Card Title";
var defaultBody = "I am a very simple card. I am good at containing small bits of information, I am convenient because I require little markup to use effectively.";
var defaultLink = "#";
Vue.component('editable', {
    template: '<span v-bind:contenteditable="state" @input="update"></span>',
    props: ['content', 'ckey', 'state'],
    mounted: function () {
        this.$el.innerText = this.content[this.ckey];
    },
    methods: {
        update: function (event) {
            this.content[this.ckey] = event.target.innerText;
        }
    }
});
Vue.component('card', {
    template: '<div v-on:mouseover="contextme" class="col s12 m3 lmdd-block" :class="{context:isContext,edit:isEdit}"><div :class="card.color" class="card"><control v-if="isEdit" @delete="removeme" @invert="invertme"></control><div class="card-content" :class="isWhite"><editable class="card-title" :state="isEdit" :content="card" ckey="title"></editable><editable :state="isEdit" :content="card" ckey="body"></editable></div><div class="card-action"><a href="#">This is a link</a><a href="#">This is a link</a></div></div></div>',
    props: ['card', 'index'],
    computed: {
        isWhite: function () {
            return (this.card.white) ? 'white-text' : ''
        },
        isContext: function () {
            return (this.$root.$data.context === this);
        },
        isEdit: function () {
            return (this.$root.$data.editMode && this.isContext)
        }
    },
    methods: {
        invertme: function () {
            this.card.white = !this.card.white;
        },
        contextme: function () {
            this.$emit('context', this);
        },
        removeme: function () {
            this.$emit('removecard', this.index);
        }
    }
});
Vue.component('control', {
    template: '<div class="control">' +
        '<a @click="invert" class="btn-floating waves-effect waves-light black"><i class="material-icons">invert_colors</i></a>' +
        '<a href="#colorpalette" class="btn-floating waves-effect waves-light black"><i class="material-icons">palette</i></a>' +
        '<a @click="$emit(`delete`)"class="btn-floating waves-effect waves-light black"><i class="material-icons">delete_forever</i></a>' +
        '<a class="btn-floating waves-effect waves-light black"><i class="material-icons handle">open_with</i></a>' +
        '</div>',
    methods: {
        invert: function () {
            this.$emit('invert');
        }
    }
});
Vue.component('panel', {
    template: '<div class="panel" :style="style"><a class="btn-floating waves-effect waves-light black" :class="{pulse:!edit}" @click="toggleme"><i class="material-icons">build</i></a></div>',
    props: ['edit'],
    methods: {
        toggleme: function () {
            this.$emit('switch', this);
        }
    },
    computed: {
        style: function () {
            return {
                position: "absolute",
                transform: "translate3d(" + this.$root.rect.left + "," + this.$root.rect.top + ",0px)"
            }
        }
    }
});
var app = new Vue({
    el: '#app',
    data: {
        rect: {
            top: '',
            left: ''
        },
        editMode: false,
        dragEvent: false,
        context: {},
        cards: [],
        colors: ['purple', 'indigo', 'blue', 'green', 'yellow', 'orange', 'red'],
        colorVariants: ['darken-4', 'darken-3', 'darken-2', 'darken-1', '', 'lighten-1', 'lighten-2', 'lighten-3', 'lighten-4']
    },
    mounted: function () {
        lmdd.set(document.getElementById('lmdd-scope'), {
            containerClass: 'row',
            draggableItemClass: 'col',
            handleClass: 'handle',
            dataMode: true
        });
        this.$el.addEventListener('lmddend', this.handleDragEnd);
        this.$el.addEventListener('lmddstart', this.handleDragStart);
    },
    methods: {
        handleDragStart: function () {
            this.dragEvent = true;
        },
        handleDragEnd: function (event) {
            if (event.detail.to) {
                var newIndex = event.detail.to.index - 1;
                var oldIndex = event.detail.from.index - 1;
                this.cards.splice(newIndex, 0, this.cards.splice(oldIndex, 1)[0]);
                var component = event.detail.draggedElement.__vue__;
                this.context = component;
                this.rect.top = component.$el.getBoundingClientRect().top + window.pageYOffset - 15 + 'px';
                this.rect.left = component.$el.getBoundingClientRect().left + window.pageXOffset - 15 + 'px';
            }

            this.dragEvent = false;
        },
        updatecolor: function (color, variant) {
            this.context.card.color = color + ' ' + variant;
        },
        resetpanel: function () {
            this.editMode = false;
            this.context = {};
            this.rect.top = '-99999px';
        },
        removecard: function (event) {
            this.cards.splice(event, 1);
            this.resetpanel();
        },
        addNewCard: function () {
            this.cards.push({
                title: defaultTitle,
                body: defaultBody,
                white: true,
                color: this.colors[Math.floor(Math.random() * 5)]
            });
        },
        apply: function (component) {
            if ((this.context !== component) && (!this.editMode)) {
                this.context = component;
                this.rect.top = component.$el.getBoundingClientRect().top + window.pageYOffset - 15 + 'px';
                this.rect.left = component.$el.getBoundingClientRect().left + window.pageXOffset - 15 + 'px';
            }
        }
    }
});
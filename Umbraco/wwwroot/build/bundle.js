(function () {
    'use strict';

    function noop() { }
    function run(fn) {
        return fn();
    }
    function blank_object() {
        return Object.create(null);
    }
    function run_all(fns) {
        fns.forEach(run);
    }
    function is_function(thing) {
        return typeof thing === 'function';
    }
    function safe_not_equal(a, b) {
        return a != a ? b == b : a !== b || ((a && typeof a === 'object') || typeof a === 'function');
    }
    let src_url_equal_anchor;
    function src_url_equal(element_src, url) {
        if (!src_url_equal_anchor) {
            src_url_equal_anchor = document.createElement('a');
        }
        src_url_equal_anchor.href = url;
        return element_src === src_url_equal_anchor.href;
    }
    function is_empty(obj) {
        return Object.keys(obj).length === 0;
    }
    function append(target, node) {
        target.appendChild(node);
    }
    function insert(target, node, anchor) {
        target.insertBefore(node, anchor || null);
    }
    function detach(node) {
        node.parentNode.removeChild(node);
    }
    function destroy_each(iterations, detaching) {
        for (let i = 0; i < iterations.length; i += 1) {
            if (iterations[i])
                iterations[i].d(detaching);
        }
    }
    function element(name) {
        return document.createElement(name);
    }
    function text(data) {
        return document.createTextNode(data);
    }
    function space() {
        return text(' ');
    }
    function attr(node, attribute, value) {
        if (value == null)
            node.removeAttribute(attribute);
        else if (node.getAttribute(attribute) !== value)
            node.setAttribute(attribute, value);
    }
    function children(element) {
        return Array.from(element.childNodes);
    }
    function set_data(text, data) {
        data = '' + data;
        if (text.wholeText !== data)
            text.data = data;
    }
    function attribute_to_object(attributes) {
        const result = {};
        for (const attribute of attributes) {
            result[attribute.name] = attribute.value;
        }
        return result;
    }

    let current_component;
    function set_current_component(component) {
        current_component = component;
    }
    function get_current_component() {
        if (!current_component)
            throw new Error('Function called outside component initialization');
        return current_component;
    }
    /**
     * The `onMount` function schedules a callback to run as soon as the component has been mounted to the DOM.
     * It must be called during the component's initialisation (but doesn't need to live *inside* the component;
     * it can be called from an external module).
     *
     * `onMount` does not run inside a [server-side component](/docs#run-time-server-side-component-api).
     *
     * https://svelte.dev/docs#run-time-svelte-onmount
     */
    function onMount(fn) {
        get_current_component().$$.on_mount.push(fn);
    }

    const dirty_components = [];
    const binding_callbacks = [];
    const render_callbacks = [];
    const flush_callbacks = [];
    const resolved_promise = Promise.resolve();
    let update_scheduled = false;
    function schedule_update() {
        if (!update_scheduled) {
            update_scheduled = true;
            resolved_promise.then(flush);
        }
    }
    function add_render_callback(fn) {
        render_callbacks.push(fn);
    }
    // flush() calls callbacks in this order:
    // 1. All beforeUpdate callbacks, in order: parents before children
    // 2. All bind:this callbacks, in reverse order: children before parents.
    // 3. All afterUpdate callbacks, in order: parents before children. EXCEPT
    //    for afterUpdates called during the initial onMount, which are called in
    //    reverse order: children before parents.
    // Since callbacks might update component values, which could trigger another
    // call to flush(), the following steps guard against this:
    // 1. During beforeUpdate, any updated components will be added to the
    //    dirty_components array and will cause a reentrant call to flush(). Because
    //    the flush index is kept outside the function, the reentrant call will pick
    //    up where the earlier call left off and go through all dirty components. The
    //    current_component value is saved and restored so that the reentrant call will
    //    not interfere with the "parent" flush() call.
    // 2. bind:this callbacks cannot trigger new flush() calls.
    // 3. During afterUpdate, any updated components will NOT have their afterUpdate
    //    callback called a second time; the seen_callbacks set, outside the flush()
    //    function, guarantees this behavior.
    const seen_callbacks = new Set();
    let flushidx = 0; // Do *not* move this inside the flush() function
    function flush() {
        const saved_component = current_component;
        do {
            // first, call beforeUpdate functions
            // and update components
            while (flushidx < dirty_components.length) {
                const component = dirty_components[flushidx];
                flushidx++;
                set_current_component(component);
                update(component.$$);
            }
            set_current_component(null);
            dirty_components.length = 0;
            flushidx = 0;
            while (binding_callbacks.length)
                binding_callbacks.pop()();
            // then, once components are updated, call
            // afterUpdate functions. This may cause
            // subsequent updates...
            for (let i = 0; i < render_callbacks.length; i += 1) {
                const callback = render_callbacks[i];
                if (!seen_callbacks.has(callback)) {
                    // ...so guard against infinite loops
                    seen_callbacks.add(callback);
                    callback();
                }
            }
            render_callbacks.length = 0;
        } while (dirty_components.length);
        while (flush_callbacks.length) {
            flush_callbacks.pop()();
        }
        update_scheduled = false;
        seen_callbacks.clear();
        set_current_component(saved_component);
    }
    function update($$) {
        if ($$.fragment !== null) {
            $$.update();
            run_all($$.before_update);
            const dirty = $$.dirty;
            $$.dirty = [-1];
            $$.fragment && $$.fragment.p($$.ctx, dirty);
            $$.after_update.forEach(add_render_callback);
        }
    }
    const outroing = new Set();
    function transition_in(block, local) {
        if (block && block.i) {
            outroing.delete(block);
            block.i(local);
        }
    }
    function mount_component(component, target, anchor, customElement) {
        const { fragment, after_update } = component.$$;
        fragment && fragment.m(target, anchor);
        if (!customElement) {
            // onMount happens before the initial afterUpdate
            add_render_callback(() => {
                const new_on_destroy = component.$$.on_mount.map(run).filter(is_function);
                // if the component was destroyed immediately
                // it will update the `$$.on_destroy` reference to `null`.
                // the destructured on_destroy may still reference to the old array
                if (component.$$.on_destroy) {
                    component.$$.on_destroy.push(...new_on_destroy);
                }
                else {
                    // Edge case - component was destroyed immediately,
                    // most likely as a result of a binding initialising
                    run_all(new_on_destroy);
                }
                component.$$.on_mount = [];
            });
        }
        after_update.forEach(add_render_callback);
    }
    function destroy_component(component, detaching) {
        const $$ = component.$$;
        if ($$.fragment !== null) {
            run_all($$.on_destroy);
            $$.fragment && $$.fragment.d(detaching);
            // TODO null out other refs, including component.$$ (but need to
            // preserve final state?)
            $$.on_destroy = $$.fragment = null;
            $$.ctx = [];
        }
    }
    function make_dirty(component, i) {
        if (component.$$.dirty[0] === -1) {
            dirty_components.push(component);
            schedule_update();
            component.$$.dirty.fill(0);
        }
        component.$$.dirty[(i / 31) | 0] |= (1 << (i % 31));
    }
    function init(component, options, instance, create_fragment, not_equal, props, append_styles, dirty = [-1]) {
        const parent_component = current_component;
        set_current_component(component);
        const $$ = component.$$ = {
            fragment: null,
            ctx: [],
            // state
            props,
            update: noop,
            not_equal,
            bound: blank_object(),
            // lifecycle
            on_mount: [],
            on_destroy: [],
            on_disconnect: [],
            before_update: [],
            after_update: [],
            context: new Map(options.context || (parent_component ? parent_component.$$.context : [])),
            // everything else
            callbacks: blank_object(),
            dirty,
            skip_bound: false,
            root: options.target || parent_component.$$.root
        };
        append_styles && append_styles($$.root);
        let ready = false;
        $$.ctx = instance
            ? instance(component, options.props || {}, (i, ret, ...rest) => {
                const value = rest.length ? rest[0] : ret;
                if ($$.ctx && not_equal($$.ctx[i], $$.ctx[i] = value)) {
                    if (!$$.skip_bound && $$.bound[i])
                        $$.bound[i](value);
                    if (ready)
                        make_dirty(component, i);
                }
                return ret;
            })
            : [];
        $$.update();
        ready = true;
        run_all($$.before_update);
        // `false` as a special case of no DOM component
        $$.fragment = create_fragment ? create_fragment($$.ctx) : false;
        if (options.target) {
            if (options.hydrate) {
                const nodes = children(options.target);
                // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                $$.fragment && $$.fragment.l(nodes);
                nodes.forEach(detach);
            }
            else {
                // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                $$.fragment && $$.fragment.c();
            }
            if (options.intro)
                transition_in(component.$$.fragment);
            mount_component(component, options.target, options.anchor, options.customElement);
            flush();
        }
        set_current_component(parent_component);
    }
    let SvelteElement;
    if (typeof HTMLElement === 'function') {
        SvelteElement = class extends HTMLElement {
            constructor() {
                super();
                this.attachShadow({ mode: 'open' });
            }
            connectedCallback() {
                const { on_mount } = this.$$;
                this.$$.on_disconnect = on_mount.map(run).filter(is_function);
                // @ts-ignore todo: improve typings
                for (const key in this.$$.slotted) {
                    // @ts-ignore todo: improve typings
                    this.appendChild(this.$$.slotted[key]);
                }
            }
            attributeChangedCallback(attr, _oldValue, newValue) {
                this[attr] = newValue;
            }
            disconnectedCallback() {
                run_all(this.$$.on_disconnect);
            }
            $destroy() {
                destroy_component(this, 1);
                this.$destroy = noop;
            }
            $on(type, callback) {
                // TODO should this delegate to addEventListener?
                if (!is_function(callback)) {
                    return noop;
                }
                const callbacks = (this.$$.callbacks[type] || (this.$$.callbacks[type] = []));
                callbacks.push(callback);
                return () => {
                    const index = callbacks.indexOf(callback);
                    if (index !== -1)
                        callbacks.splice(index, 1);
                };
            }
            $set($$props) {
                if (this.$$set && !is_empty($$props)) {
                    this.$$.skip_bound = true;
                    this.$$set($$props);
                    this.$$.skip_bound = false;
                }
            }
        };
    }

    /* wwwroot\src\Elements\Button.svelte generated by Svelte v3.52.0 */

    function create_fragment$3(ctx) {
    	let t0;
    	let main;
    	let h1;
    	let t1;
    	let t2;
    	let t3;

    	return {
    		c() {
    			t0 = space();
    			main = element("main");
    			h1 = element("h1");
    			t1 = text("Hello ");
    			t2 = text(/*name*/ ctx[0]);
    			t3 = text("!");
    			this.c = noop;
    			attr(h1, "id", /*id*/ ctx[1]);
    		},
    		m(target, anchor) {
    			insert(target, t0, anchor);
    			insert(target, main, anchor);
    			append(main, h1);
    			append(h1, t1);
    			append(h1, t2);
    			append(h1, t3);
    		},
    		p(ctx, [dirty]) {
    			if (dirty & /*name*/ 1) set_data(t2, /*name*/ ctx[0]);

    			if (dirty & /*id*/ 2) {
    				attr(h1, "id", /*id*/ ctx[1]);
    			}
    		},
    		i: noop,
    		o: noop,
    		d(detaching) {
    			if (detaching) detach(t0);
    			if (detaching) detach(main);
    		}
    	};
    }

    function instance$3($$self, $$props, $$invalidate) {
    	let { name } = $$props;
    	let { id } = $$props;

    	$$self.$$set = $$props => {
    		if ('name' in $$props) $$invalidate(0, name = $$props.name);
    		if ('id' in $$props) $$invalidate(1, id = $$props.id);
    	};

    	return [name, id];
    }

    class Button extends SvelteElement {
    	constructor(options) {
    		super();
    		this.shadowRoot.innerHTML = `<style>h1{font-size:5em}</style>`;

    		init(
    			this,
    			{
    				target: this.shadowRoot,
    				props: attribute_to_object(this.attributes),
    				customElement: true
    			},
    			instance$3,
    			create_fragment$3,
    			safe_not_equal,
    			{ name: 0, id: 1 },
    			null
    		);

    		if (options) {
    			if (options.target) {
    				insert(options.target, this, options.anchor);
    			}

    			if (options.props) {
    				this.$set(options.props);
    				flush();
    			}
    		}
    	}

    	static get observedAttributes() {
    		return ["name", "id"];
    	}

    	get name() {
    		return this.$$.ctx[0];
    	}

    	set name(name) {
    		this.$$set({ name });
    		flush();
    	}

    	get id() {
    		return this.$$.ctx[1];
    	}

    	set id(id) {
    		this.$$set({ id });
    		flush();
    	}
    }

    customElements.define("uw-button", Button);

    /* wwwroot\src\Elements\HomeHero.svelte generated by Svelte v3.52.0 */

    function get_each_context(ctx, list, i) {
    	const child_ctx = ctx.slice();
    	child_ctx[3] = list[i];
    	child_ctx[5] = i;
    	return child_ctx;
    }

    // (81:4) {#each Array(30) as _, i}
    function create_each_block(ctx) {
    	let img;
    	let img_src_value;

    	return {
    		c() {
    			img = element("img");
    			if (!src_url_equal(img.src, img_src_value = "/assets/images/Star.svg")) attr(img, "src", img_src_value);
    			attr(img, "class", "star");
    			attr(img, "alt", "SVG Star");
    		},
    		m(target, anchor) {
    			insert(target, img, anchor);
    		},
    		p: noop,
    		d(detaching) {
    			if (detaching) detach(img);
    		}
    	};
    }

    function create_fragment$2(ctx) {
    	let t0;
    	let div1;
    	let img0;
    	let img0_src_value;
    	let t1;
    	let img1;
    	let img1_src_value;
    	let t2;
    	let t3;
    	let div0;
    	let each_value = Array(30);
    	let each_blocks = [];

    	for (let i = 0; i < each_value.length; i += 1) {
    		each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
    	}

    	return {
    		c() {
    			t0 = space();
    			div1 = element("div");
    			img0 = element("img");
    			t1 = space();
    			img1 = element("img");
    			t2 = space();

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].c();
    			}

    			t3 = space();
    			div0 = element("div");

    			div0.innerHTML = `<h1 class="home-hero--heading"><slot name="heading"></slot></h1> 
        <p><slot name="subheading"></slot></p>`;

    			this.c = noop;
    			attr(img0, "alt", "Large peach planet");
    			attr(img0, "class", "planet--large");
    			if (!src_url_equal(img0.src, img0_src_value = "/assets/images/planetLarge.svg")) attr(img0, "src", img0_src_value);
    			attr(img1, "alt", "Small blue planet with moon");
    			attr(img1, "class", "planet--small");
    			if (!src_url_equal(img1.src, img1_src_value = "/assets/images/planetSmall.svg")) attr(img1, "src", img1_src_value);
    			attr(div0, "class", "home-hero--inner");
    			attr(div1, "class", "home-hero");
    		},
    		m(target, anchor) {
    			insert(target, t0, anchor);
    			insert(target, div1, anchor);
    			append(div1, img0);
    			append(div1, t1);
    			append(div1, img1);
    			append(div1, t2);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].m(div1, null);
    			}

    			append(div1, t3);
    			append(div1, div0);
    			/*div1_binding*/ ctx[1](div1);
    		},
    		p: noop,
    		i: noop,
    		o: noop,
    		d(detaching) {
    			if (detaching) detach(t0);
    			if (detaching) detach(div1);
    			destroy_each(each_blocks, detaching);
    			/*div1_binding*/ ctx[1](null);
    		}
    	};
    }

    const TRIES_PER_BOX = 50;

    function randomIntFromInterval(min, max) {
    	return Math.floor(Math.random() * (max - min + 1) + min);
    }

    function instance$2($$self, $$props, $$invalidate) {
    	let root;
    	const randUint = range => Math.random() * range | 0;

    	onMount(() => {
    		const placing = [...root.querySelectorAll(".star")].map(el => Bounds(el, 5));
    		const fitted = [];
    		const areaToFit = Bounds();
    		let maxTries = TRIES_PER_BOX * placing.length;

    		while (placing.length && maxTries > 0) {
    			let i = 0;

    			while (i < placing.length) {
    				const box = placing[i];
    				box.moveTo(randUint(areaToFit.w - box.w), randUint(areaToFit.h - box.h));

    				if (fitted.every(placed => !placed.overlaps(box))) {
    					fitted.push(placing.splice(i--, 1)[0].placeElement());
    				} else {
    					maxTries--;
    				}

    				i++;
    			}
    		}

    		function Bounds(el, pad = 0) {
    			const box = el?.getBoundingClientRect() ?? {
    				left: 0,
    				top: 0,
    				right: innerWidth,
    				bottom: innerHeight,
    				width: innerWidth,
    				height: innerHeight
    			};

    			return {
    				l: box.left - pad,
    				t: box.top - pad,
    				r: box.right + pad,
    				b: box.bottom + pad,
    				w: box.width + pad * 2,
    				h: box.height + pad * 2,
    				overlaps(bounds) {
    					return !(this.l > bounds.r || this.r < bounds.l || this.t > bounds.b || this.b < bounds.t);
    				},
    				moveTo(x, y) {
    					this.r = (this.l = x) + this.w;
    					this.b = (this.t = y) + this.h;
    					return this;
    				},
    				placeElement() {
    					if (el) {
    						el.style.top = this.t + pad + "px";
    						el.style.left = this.l + pad + "px";
    						el.style.width = randomIntFromInterval(12, 30) + "px";
    						el.style.transform = 'rotate(' + randomIntFromInterval(0, 180) + "deg)";
    						el.classList.add("placed");
    					}

    					return this;
    				}
    			};
    		}
    	});

    	function div1_binding($$value) {
    		binding_callbacks[$$value ? 'unshift' : 'push'](() => {
    			root = $$value;
    			$$invalidate(0, root);
    		});
    	}

    	return [root, div1_binding];
    }

    class HomeHero extends SvelteElement {
    	constructor(options) {
    		super();
    		this.shadowRoot.innerHTML = `<style>:root{--color-white:#ffffff;--color-black:#000000;--color-primary:#3544B1;--color-peach-light:#F5C1BC;--color-peach-dark:#E89199}@media only screen and (min-width: 33.75em){}@media only screen and (min-width: 60em){}@media only screen and (min-width: 45em){}@media(max-width: 600px){}*,*::before,*::after{-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box}@media(prefers-reduced-motion: no-preference){:root{scroll-behavior:smooth}}.home-hero{background:linear-gradient(180deg, rgba(14, 21, 68, 0.48) 0%, rgba(53, 68, 177, 0) 100%);height:100vh;color:var(--color-white);overflow:hidden}.home-hero--heading{font-size:clamp(3rem, 10vw, 5.625rem);max-width:15ch;line-height:clamp(60px, 10vw, 101.5px);padding:1rem;z-index:999}.home-hero--inner{text-align:center;display:flex;flex-direction:column;justify-content:center;align-items:center;height:100%;width:100%}.home-hero--inner p{max-width:50ch;z-index:999}.planet--large{position:absolute;top:90%;left:0;transform:translate(0%, -90%);width:clamp(12rem, 25vw, 20rem);z-index:99}@media only screen and (max-width: 600px){.planet--large{top:100%;transform:translate(0%, -100%)}}.planet--small{position:absolute;top:30%;right:0;transform:translate(0%, -30%);width:clamp(7rem, 10vw, 20rem);z-index:99}@media only screen and (max-width: 600px){.planet--small{top:15%;transform:translate(0%, -15%)}}.star{position:absolute;animation:grow 2s infinite;z-index:9}@keyframes grow{0%,100%{transform:scale(1)}50%{transform:scale(1.3)}}</style>`;

    		init(
    			this,
    			{
    				target: this.shadowRoot,
    				props: attribute_to_object(this.attributes),
    				customElement: true
    			},
    			instance$2,
    			create_fragment$2,
    			safe_not_equal,
    			{},
    			null
    		);

    		if (options) {
    			if (options.target) {
    				insert(options.target, this, options.anchor);
    			}
    		}
    	}
    }

    customElements.define("uw-home-hero", HomeHero);

    /* wwwroot\src\Elements\Section.svelte generated by Svelte v3.52.0 */

    function create_fragment$1(ctx) {
    	let t;
    	let section;
    	let div;
    	let section_class_value;

    	return {
    		c() {
    			t = space();
    			section = element("section");
    			div = element("div");
    			div.innerHTML = `<slot></slot>`;
    			this.c = noop;
    			attr(div, "class", "section--inner");
    			attr(section, "class", section_class_value = "bg-" + /*type*/ ctx[0]);
    		},
    		m(target, anchor) {
    			insert(target, t, anchor);
    			insert(target, section, anchor);
    			append(section, div);
    		},
    		p(ctx, [dirty]) {
    			if (dirty & /*type*/ 1 && section_class_value !== (section_class_value = "bg-" + /*type*/ ctx[0])) {
    				attr(section, "class", section_class_value);
    			}
    		},
    		i: noop,
    		o: noop,
    		d(detaching) {
    			if (detaching) detach(t);
    			if (detaching) detach(section);
    		}
    	};
    }

    function instance$1($$self, $$props, $$invalidate) {
    	let { type } = $$props;

    	$$self.$$set = $$props => {
    		if ('type' in $$props) $$invalidate(0, type = $$props.type);
    	};

    	return [type];
    }

    class Section extends SvelteElement {
    	constructor(options) {
    		super();
    		this.shadowRoot.innerHTML = `<style>:root{--color-white:#ffffff;--color-black:#000000;--color-primary:#3544B1;--color-peach-light:#F5C1BC;--color-peach-dark:#E89199}.text-center{text-align:center}.text-uppercase{text-transform:uppercase}.text-white{color:#ffffff}.text-black{color:#000000}.text-primary{color:#3544B1}.text-peach-light{color:#F5C1BC}.text-peach-dark{color:#E89199}.bg-white{background:#ffffff}.bg-black{background:#000000}.bg-primary{background:#3544B1}.bg-peach-light{background:#F5C1BC}.bg-peach-dark{background:#E89199}.container{width:90%;margin-left:auto;margin-right:auto}@media only screen and (min-width: 33.75em){.container{width:80%}}@media only screen and (min-width: 60em){.container{width:75%;max-width:60rem}}.row{position:relative;width:100%}.row::after{content:"";display:table;clear:both}.col-1,.col-2,.col-3,.col-4,.col-5,.col-6,.col-7,.col-8,.col-9,.col-10,.col-11,.col-12{width:96%}.col-1-sm{width:4.33333%}.col-2-sm{width:12.66667%}.col-3-sm{width:21%}.col-4-sm{width:29.33333%}.col-5-sm{width:37.66667%}.col-6-sm{width:46%}.col-7-sm{width:54.33333%}.col-8-sm{width:62.66667%}.col-9-sm{width:71%}.col-10-sm{width:79.33333%}.col-11-sm{width:87.66667%}.col-12-sm{width:96%}@media only screen and (min-width: 45em){.col-1{width:4.33333%}.col-2{width:12.66667%}.col-3{width:21%}.col-4{width:29.33333%}.col-5{width:37.66667%}.col-6{width:46%}.col-7{width:54.33333%}.col-8{width:62.66667%}.col-9{width:71%}.col-10{width:79.33333%}.col-11{width:87.66667%}.col-12{width:96%}.hidden-sm{display:block}}.navbar{position:absolute;width:100vw;top:0;left:0;padding:2rem 5vw;background:transparent}.navbar__inner{display:flex;flex-direction:row;flex-wrap:wrap;align-items:center;justify-content:space-between;margin-inline:auto}.navbar__brand{color:var(--color-white);text-decoration:none;font-size:clamp(1.5rem, 5vw, 2rem);font-weight:800}.burger{background:none;border:none}.burger-bar,.burger-bar{width:35px;height:6px;border-radius:50px;margin:6px 0;transition:0.4s;background-color:var(--color-white)}.burger-bar--invisible,.burger-bar--invisible{width:35px;height:2px;margin:6px 0}.home-showcases--container{max-width:60%;margin-inline:auto;display:grid;grid-template-columns:repeat(2, 1fr);grid-template-rows:repeat(2, 1fr);gap:1rem}@media(max-width: 600px){.home-showcases--container{max-width:100%;grid-template-columns:repeat(1, 1fr)}}*,*::before,*::after{-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box}@media(prefers-reduced-motion: no-preference){:root{scroll-behavior:smooth}}.section--inner{margin-left:102px;margin-right:102px;padding-top:5.66667rem;padding-bottom:5.66667rem}@media(max-width: 600px){.section--inner{margin-left:30px;margin-right:30px}}</style>`;

    		init(
    			this,
    			{
    				target: this.shadowRoot,
    				props: attribute_to_object(this.attributes),
    				customElement: true
    			},
    			instance$1,
    			create_fragment$1,
    			safe_not_equal,
    			{ type: 0 },
    			null
    		);

    		if (options) {
    			if (options.target) {
    				insert(options.target, this, options.anchor);
    			}

    			if (options.props) {
    				this.$set(options.props);
    				flush();
    			}
    		}
    	}

    	static get observedAttributes() {
    		return ["type"];
    	}

    	get type() {
    		return this.$$.ctx[0];
    	}

    	set type(type) {
    		this.$$set({ type });
    		flush();
    	}
    }

    customElements.define("uw-section", Section);

    /* wwwroot\src\Elements\Card.svelte generated by Svelte v3.52.0 */

    function create_fragment(ctx) {
    	let t0;
    	let div6;
    	let div0;
    	let img0;
    	let img0_src_value;
    	let t1;
    	let div1;
    	let h3;
    	let t2;
    	let t3;
    	let div2;
    	let t4;
    	let t5;
    	let div5;
    	let div3;
    	let img1;
    	let img1_src_value;
    	let img1_alt_value;
    	let t6;
    	let t7;
    	let t8;
    	let div4;
    	let t9;

    	return {
    		c() {
    			t0 = space();
    			div6 = element("div");
    			div0 = element("div");
    			img0 = element("img");
    			t1 = space();
    			div1 = element("div");
    			h3 = element("h3");
    			t2 = text(/*title*/ ctx[1]);
    			t3 = space();
    			div2 = element("div");
    			t4 = text(/*body*/ ctx[2]);
    			t5 = space();
    			div5 = element("div");
    			div3 = element("div");
    			img1 = element("img");
    			t6 = space();
    			t7 = text(/*author_name*/ ctx[4]);
    			t8 = space();
    			div4 = element("div");
    			t9 = text(/*date*/ ctx[5]);
    			this.c = noop;
    			if (!src_url_equal(img0.src, img0_src_value = /*image_src*/ ctx[0])) attr(img0, "src", img0_src_value);
    			attr(img0, "alt", /*title*/ ctx[1]);
    			attr(img0, "width", "500");
    			attr(img0, "height", "300");
    			attr(div0, "class", "card--image");
    			attr(div1, "class", "card--title");
    			attr(div2, "class", "card--body");
    			if (!src_url_equal(img1.src, img1_src_value = /*profile_src*/ ctx[3])) attr(img1, "src", img1_src_value);
    			attr(img1, "alt", img1_alt_value = "" + (/*author_name*/ ctx[4] + " profile picture"));
    			attr(img1, "width", "40");
    			attr(img1, "height", "40");
    			attr(div3, "class", "card--meta__author");
    			attr(div4, "class", "card--meta__date");
    			attr(div5, "class", "card--meta text-primary text-uppercase");
    			attr(div6, "class", "card");
    		},
    		m(target, anchor) {
    			insert(target, t0, anchor);
    			insert(target, div6, anchor);
    			append(div6, div0);
    			append(div0, img0);
    			append(div6, t1);
    			append(div6, div1);
    			append(div1, h3);
    			append(h3, t2);
    			append(div6, t3);
    			append(div6, div2);
    			append(div2, t4);
    			append(div6, t5);
    			append(div6, div5);
    			append(div5, div3);
    			append(div3, img1);
    			append(div3, t6);
    			append(div3, t7);
    			append(div5, t8);
    			append(div5, div4);
    			append(div4, t9);
    		},
    		p(ctx, [dirty]) {
    			if (dirty & /*image_src*/ 1 && !src_url_equal(img0.src, img0_src_value = /*image_src*/ ctx[0])) {
    				attr(img0, "src", img0_src_value);
    			}

    			if (dirty & /*title*/ 2) {
    				attr(img0, "alt", /*title*/ ctx[1]);
    			}

    			if (dirty & /*title*/ 2) set_data(t2, /*title*/ ctx[1]);
    			if (dirty & /*body*/ 4) set_data(t4, /*body*/ ctx[2]);

    			if (dirty & /*profile_src*/ 8 && !src_url_equal(img1.src, img1_src_value = /*profile_src*/ ctx[3])) {
    				attr(img1, "src", img1_src_value);
    			}

    			if (dirty & /*author_name*/ 16 && img1_alt_value !== (img1_alt_value = "" + (/*author_name*/ ctx[4] + " profile picture"))) {
    				attr(img1, "alt", img1_alt_value);
    			}

    			if (dirty & /*author_name*/ 16) set_data(t7, /*author_name*/ ctx[4]);
    			if (dirty & /*date*/ 32) set_data(t9, /*date*/ ctx[5]);
    		},
    		i: noop,
    		o: noop,
    		d(detaching) {
    			if (detaching) detach(t0);
    			if (detaching) detach(div6);
    		}
    	};
    }

    function instance($$self, $$props, $$invalidate) {
    	let { image_src } = $$props;
    	let { title } = $$props;
    	let { body } = $$props;
    	let { profile_src } = $$props;
    	let { author_name } = $$props;
    	let { date } = $$props;

    	$$self.$$set = $$props => {
    		if ('image_src' in $$props) $$invalidate(0, image_src = $$props.image_src);
    		if ('title' in $$props) $$invalidate(1, title = $$props.title);
    		if ('body' in $$props) $$invalidate(2, body = $$props.body);
    		if ('profile_src' in $$props) $$invalidate(3, profile_src = $$props.profile_src);
    		if ('author_name' in $$props) $$invalidate(4, author_name = $$props.author_name);
    		if ('date' in $$props) $$invalidate(5, date = $$props.date);
    	};

    	return [image_src, title, body, profile_src, author_name, date];
    }

    class Card extends SvelteElement {
    	constructor(options) {
    		super();
    		this.shadowRoot.innerHTML = `<style>:root{--color-white:#ffffff;--color-black:#000000;--color-primary:#3544B1;--color-peach-light:#F5C1BC;--color-peach-dark:#E89199}.text-uppercase{text-transform:uppercase}.text-primary{color:#3544B1}@media only screen and (min-width: 33.75em){}@media only screen and (min-width: 60em){}@media only screen and (min-width: 45em){}@media(max-width: 600px){}*,*::before,*::after{-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box}@media(prefers-reduced-motion: no-preference){:root{scroll-behavior:smooth}}.card{background:var(--color-white);border-radius:20px;padding:1rem}.card--image img{border-radius:1rem;width:100%}.card--body{font-size:0.88889rem}.card--title{color:var(--color-primary)}.card--meta{display:flex;justify-content:space-between;align-items:center;margin-top:2rem;font-weight:700;font-size:0.88889rem}.card--meta__author{display:flex;align-items:center;gap:1rem}.card--meta img{max-width:40px;border-radius:100px}</style>`;

    		init(
    			this,
    			{
    				target: this.shadowRoot,
    				props: attribute_to_object(this.attributes),
    				customElement: true
    			},
    			instance,
    			create_fragment,
    			safe_not_equal,
    			{
    				image_src: 0,
    				title: 1,
    				body: 2,
    				profile_src: 3,
    				author_name: 4,
    				date: 5
    			},
    			null
    		);

    		if (options) {
    			if (options.target) {
    				insert(options.target, this, options.anchor);
    			}

    			if (options.props) {
    				this.$set(options.props);
    				flush();
    			}
    		}
    	}

    	static get observedAttributes() {
    		return ["image_src", "title", "body", "profile_src", "author_name", "date"];
    	}

    	get image_src() {
    		return this.$$.ctx[0];
    	}

    	set image_src(image_src) {
    		this.$$set({ image_src });
    		flush();
    	}

    	get title() {
    		return this.$$.ctx[1];
    	}

    	set title(title) {
    		this.$$set({ title });
    		flush();
    	}

    	get body() {
    		return this.$$.ctx[2];
    	}

    	set body(body) {
    		this.$$set({ body });
    		flush();
    	}

    	get profile_src() {
    		return this.$$.ctx[3];
    	}

    	set profile_src(profile_src) {
    		this.$$set({ profile_src });
    		flush();
    	}

    	get author_name() {
    		return this.$$.ctx[4];
    	}

    	set author_name(author_name) {
    		this.$$set({ author_name });
    		flush();
    	}

    	get date() {
    		return this.$$.ctx[5];
    	}

    	set date(date) {
    		this.$$set({ date });
    		flush();
    	}
    }

    customElements.define("uw-card", Card);

})();
